using Microsoft.AspNetCore.Mvc;
using Quizer.Models;
using Microsoft.Extensions.Options;
using Quizer.Context;
using Microsoft.AspNetCore.Authorization;
using QuizerServer.HelperClasses;
using Microsoft.EntityFrameworkCore;

namespace Quizer.Controllers;

[Route("Authorization")]
public class Authorization : Controller
{

    private readonly JwtSettings _options;
    private string? _token;
    private readonly ILogger<Authorization> _logger;
    private ApplicationContext _context;

    public Authorization(IOptions<JwtSettings> options, ILogger<Authorization> logger, ApplicationContext context)
    {
        _logger = logger;
        _options = options.Value;
        _context = context;
    }

    [HttpPost, Route("Auth")]
    public async Task<IActionResult>? Auth()
    {
        UsersServices usersServices = new() { db = _context };

        try
        {
            Users? userData = await HttpContext.Request.ReadFromJsonAsync<Users>();

            Users? person = await usersServices.GetEntity(new Dictionary<string, object>
            {
                    {"firstname", userData?.Firstname?.ToLower()!},
                    {"lastname", userData?.Lastname?.ToLower()!},
                    {"patronymic", userData?.Patronymic?.ToLower()!},
                    {"password", int.Parse(userData?.Password!)},
                    {"group", userData?.GroupsId ?? '1'}
            });
            
            if (person is null) {
                            
                var response = new
                {
                    status = false,
                    text = "Пользователь не найден"
                };

                return Json(response);
            }
            else
            {
                GroupsServices groupsServices = new GroupsServices { db = _context };

                List<Groups> groupsList = await groupsServices.EntityLIst();
                
                string username = $"{person?.Firstname?.Replace(" ", "")} " +
                                                    $"{person?.Lastname?.Replace(" ", "")}";

                List<Subjects> subjectsList = await _context.Subjects!.ToListAsync();
                TokenSecurity tokenSecurity = new(_options, username);

                _token = tokenSecurity.GetToken();

                Groups? groups = groupsList?.FirstOrDefault(x => x?.Id == userData?.GroupsId);

                var response = new
                {
                    accessToken = _token,
                    username = tokenSecurity._claimsCreator.GetClaims().Name,
                    group = groups?.Name,
                    groupId = groups?.Id,
                    id = person?.Id
                };

                Sessions.CreateSession(person?.Id, person?.Firstname, person?.Lastname, DateTime.Now);

                return Json(response);
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            var response = new
            {
                status = false,
                text = "Ошибка авторизации"
            };
            return Json(response);
        }
    }

    [HttpGet, Route("GetGroups")]
    public async Task<JsonResult>? GetGroups()
    {
        try
        {
            string[][]? groupsName = _context.Groups.Select(x => new string[] { x.Name!, x.Id.ToString()! }).ToArray();

            return Json(groupsName);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return Json("");
        }
    }

    [HttpPost("DetectToken")]
    [Authorize]
    public JsonResult DetectToken()
    {
        Console.WriteLine("Auth success");
        return Json(new Dictionary<string, bool> {
            {"access", true}
        });
    }

}