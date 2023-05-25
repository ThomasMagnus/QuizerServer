using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quizer.Context;
using Quizer.IServices;
using Quizer.Models;
using Quizer.RequestBody;
using QuizerServer.HelperClasses;
using System.Text.Json;
using static System.Console;
using static System.String;

namespace Quizer.Controllers
{

    public class TeacherController : Controller
    {
        private readonly JwtSettings? _jwtSettings;
        private readonly ILogger<TeacherController> _logger;
        private Lazy<TeacherPropsHelper>? _teacherPropsHelper = new Lazy<TeacherPropsHelper>();
        private ISubjectsProps _subjectsProps;
        private GroupsServices? groupsServices;
        public TeacherController(IOptions<JwtSettings> jwtSettings, ILogger<TeacherController> logger, ISubjectsProps subjectsProps) => 
            (_jwtSettings, _logger, _subjectsProps) = (jwtSettings?.Value, logger, subjectsProps);

        [HttpPost, Route("Teacher/Index")]
        public async Task<IActionResult> Index()
        {
            try
            {
                Teacher? data = await HttpContext.Request.ReadFromJsonAsync<Teacher>();

                using TeacherContext teacherContext = new();

                List<Teacher> teacherList = await teacherContext.Teachers.ToListAsync();

                Teacher? teacher = teacherList?.FirstOrDefault(x => x?.fname?.ToLower().Trim() == data?.fname?.ToLower().Trim() &&
                                                                x?.lname?.ToLower().Trim() == data?.lname?.ToLower().Trim() &&
                                                                x?.login?.ToLower().Trim() == data?.login?.ToLower().Trim() &&
                                                                x?.password == data?.password);

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

                Console.WriteLine(tokenHandler);

                _teacherPropsHelper!.Value.teacherId = teacher!.id;

                Dictionary<string, string[]> teacherProps = _teacherPropsHelper!.Value.GetTeacherProps();

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
        public IActionResult DeleteProps([FromBody] JsonElement value)
        {
            Dictionary<string, string>? data = JsonSerializer.Deserialize<Dictionary<string, string>>(value);
            if (data is null) return Json("Данные не получены");

            _teacherPropsHelper!.Value.ContextName = new ApplicationContext();

            if (HttpContext.Request.Path == "/teacher/deleteSubject") _teacherPropsHelper!.Value.DeleteSubject(data["subjectName"], int.Parse(data["userId"]));
            else if (HttpContext.Request.Path == "/teacher/deleteGroup") _teacherPropsHelper!.Value.DeleteGroup(data["groupName"], 
                data["subjectName"], int.Parse(data["userId"]));

            _teacherPropsHelper!.Value.teacherId = int.Parse(data["userId"]);

            Dictionary<string, string[]> teacherProps = _teacherPropsHelper!.Value.GetTeacherProps();

            return Json(teacherProps);
        }

        [HttpPost("teacher/addWork")]
        public async Task<IActionResult> AddWork([FromBody] JsonElement value)
        {
            AddWorkBody? data = JsonSerializer.Deserialize<AddWorkBody>(value);
            try
            {
                using TeacherPropsContext teacherPropsContext = new();
                using SubjectsContext subjectsContext = new();
                groupsServices = new() { db = new ApplicationContext() };
                
                List<Subjects>? subjectsList = await subjectsContext?.subjects?.ToListAsync()!;
                List<Groups>? groupsList = await groupsServices.EntityLIst();

                Subjects? subjects = subjectsList.FirstOrDefault(x => x.Name?.ToLower() == data?.subject?.ToLower());

                if (subjects is null) {
                    WriteLine("Предмет не найден");
                    return Json(new
                    {
                        statusCode = StatusCode(404),
                        message = "Предмет не найден!"
                    });
                };

                List<int> result = new List<int>();

                foreach (string item in data!.groups!) {
                    Groups? groups = groupsList!.FirstOrDefault(x => x.Name?.Replace(" ", "") == item.ToUpper());

                    if (groups is null) return Json(new { 
                        statusCode = StatusCode(404),
                        message = $"Группа: {item} не найдена!",
                    });

                    result.Add(groups.Id);
                }

                await teacherPropsContext.AddAsync(new TeacherProps
                {
                    teacherid = data!.teacherId,
                    subjectsid = subjects!.Id,
                    groupsid = result.ToArray(),
                });

                await teacherPropsContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                WriteLine(ex.Message);
            }

            _teacherPropsHelper!.Value.ContextName = new ApplicationContext();

            _teacherPropsHelper!.Value.teacherId = int.Parse((data?.teacherId)?.ToString()!);
            Dictionary<string, string[]> teacherProps = _teacherPropsHelper!.Value.GetTeacherProps();

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
                    statusCode = StatusCode(404),
                    message = "Произошла ошибка на сервере при получении предметов!"
                });
            }
        }
    }
}
