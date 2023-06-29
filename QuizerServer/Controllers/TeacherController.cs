using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quizer.Context;
using Quizer.IServices;
using Quizer.Models;
using Quizer.RequestBody;
using QuizerServer.HelperClasses;
using QuizerServer.Requests.UsersRequests;
using System.Text.Json;
using static System.Console;
using static System.String;

namespace Quizer.Controllers
{
    public class TeacherController : Controller
    {
        private readonly JwtSettings? _jwtSettings;
        private readonly ILogger<TeacherController> _logger;
        private readonly Lazy<TeacherPropsHelper>? _teacherPropsHelper = new Lazy<TeacherPropsHelper>();
        private readonly ISubjectsProps _subjectsProps;

        private readonly ApplicationContext _context;

        public TeacherController(IOptions<JwtSettings> jwtSettings, ILogger<TeacherController> logger, ISubjectsProps subjectsProps, 
            ApplicationContext context) => 
            (_jwtSettings, _logger, _subjectsProps, _context) = (jwtSettings?.Value, logger, subjectsProps, context);

        [HttpPost, Route("Teacher/Index")]
        public async Task<IActionResult> Index([FromServices] ISender sender)
        {
            try
            {
                Teacher? data = await HttpContext.Request.ReadFromJsonAsync<Teacher>();

                Teacher? teacher = await sender.Send(new TeacherQuery(data!.fname!.ToLower().Trim(),
                                                                        data.lname!.ToLower().Trim(),
                                                                        data.login!.ToLower().Trim(),
                                                                        data.password));

                if (teacher is null)
                {
                    var response = new
                    {
                        status = false,
                        text = "Пользователь не найден"
                    };

                    return Json(response);
                }

                string username = Format("{0} {1} {2}", teacher?.lname?.Trim(), teacher?.fname?.Trim(), teacher?.pname?.Trim());
                TokenSecurity? tokenSecurity = new(_jwtSettings!, username);
                string tokenHandler = tokenSecurity?.GetToken()!;

                _teacherPropsHelper!.Value.ContextName = _context;
                _teacherPropsHelper!.Value.teacherId = teacher!.id;

                Dictionary<string, string[]> teacherProps = await _teacherPropsHelper!.Value.GetTeacherProps();

                var dataAnswer = new
                {
                    status = true,
                    accessTokenTeacher = tokenHandler,
                    username = tokenSecurity?._claimsCreator.GetClaims().Name,
                    login = teacher?.login?.Trim(),
                    id = teacher?.id,
                    props = teacherProps
                };

                Sessions.CreateSession(teacher?.id, teacher?.fname, teacher?.lname, DateTime.Now);

                return Json(dataAnswer);
            }
            catch(Exception ex)
            {
                _logger?.LogError(ex.Message);
                return Json("Ошибка на сервере");
            }
        }

        [HttpGet, Route("Teacher/TeacherPage")]
        [Authorize]
        public void TeacherPage()
        {
            WriteLine("TeacherPage");
        }

        [HttpDelete]
        [Route("teacher/deleteGroup"), Route("teacher/deleteSubject")]
        public async Task<IActionResult> DeleteProps([FromBody] JsonElement value)
        {
            Dictionary<string, string>? data = JsonSerializer.Deserialize<Dictionary<string, string>>(value);
            if (data is null) return Json("Данные не получены");

            _teacherPropsHelper!.Value.ContextName = _context;

            if (HttpContext.Request.Path == "/teacher/deleteSubject")
            {
                await _teacherPropsHelper!.Value.DeleteSubject(data["subjectName"], int.Parse(data["userId"]));
            }
            else if (HttpContext.Request.Path == "/teacher/deleteGroup")
            {
                await _teacherPropsHelper!.Value.DeleteGroup(data["groupName"], data["subjectName"], int.Parse(data["userId"]));
            }

            _teacherPropsHelper!.Value.teacherId = int.Parse(data["userId"]);

            Dictionary<string, string[]> teacherProps = await _teacherPropsHelper!.Value.GetTeacherProps();

            return Json(teacherProps);
        }

        [HttpPost("teacher/addWork")]
        public async Task<IActionResult> AddWork([FromBody] JsonElement value)
        {   
            AddWorkBody? data = JsonSerializer.Deserialize<AddWorkBody>(value);

            try
            {
                IQueryable<Subjects> subjects = _context.Subjects;
                IQueryable<Groups> groups = _context.Groups;

                Subjects? subject = await subjects.FirstOrDefaultAsync(x => x.Name!.ToLower() == data!.subject!.ToLower());

                if (subject is null) {
                    WriteLine("Предмет не найден");
                    return Json(new
                    {
                        statusCode = StatusCode(404),
                        message = "Предмет не найден!"
                    });
                };

                List<int> result = new List<int>();

                foreach (string item in data!.groups!) {
                    Groups? group = await groups!.FirstOrDefaultAsync(x => x.Name!.Replace(" ", "") == item.ToUpper());

                    if (group is null) return Json(new {
                        statusCode = StatusCode(404),
                        message = $"Группа: {item} не найдена!",
                    });

                    result.Add(group.Id);
                }

                await _context.AddAsync(new TeacherProps
                {
                    teacherid = data!.teacherId,
                    subjectsid = subject!.Id,
                    groupsid = result.ToArray(),
                });
                
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                WriteLine(ex.Message);
            }

            _teacherPropsHelper!.Value.ContextName = _context;
            _teacherPropsHelper!.Value.teacherId = int.Parse((data?.teacherId)?.ToString()!);
            Dictionary<string, string[]> teacherProps = await _teacherPropsHelper!.Value.GetTeacherProps();

            return Json(new {
                statusCode = 200,
                message = teacherProps
            });
        }

        [HttpGet("teacher/subjects")]
        public JsonResult GetSubjects()
        {
            try
            {
                List<Subjects> subjectsList = _subjectsProps.GetSubjectsList();

                return Json(new {
                    statuseCode = StatusCode(200),
                    message = subjectsList
                });
            }
            catch (Exception ex) {
                WriteLine("Неизвестная ошибка!");
                _logger.LogError("Ошибка на сервере: {0}", ex.Message);

                return Json(new { 
                    statusCode = StatusCode(500),
                    message = "Произошла ошибка на сервере при получении предметов!"
                });
            }
        }

        [HttpGet("teacher/preview")]
        public async Task<IActionResult> GetTasks(string groupName, string subjectName, int teacherId)
        {
            try
            {
                Groups? groups = await _context.Groups
                                               .FirstOrDefaultAsync(x => x.Name!.ToLower().Trim() == groupName.ToLower().Trim());

                if (groups is null) return Json("Группа не найдена!");

                Subjects? subjects = await _context.Subjects
                                                   .FirstOrDefaultAsync(x => x.Name!.ToLower().Trim() == subjectName.ToLower().Trim());
             
                if (subjects is null) return Json("Предмет не найден!");

                List<Tasks> tasks = await _context.Tasks
                                                  .Where(x => x.teacherid == teacherId && x.subjectid == subjects.Id && x.groupid == groups.Id)
                                                  .ToListAsync();

                return Json(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
                return StatusCode(500);
            }
        }
    }
}