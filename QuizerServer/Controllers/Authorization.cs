using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quizer.Context;
using Quizer.Models;
using QuizerServer.HelperClasses;
using QuizerServer.Requests.UsersRequests;

namespace Quizer.Controllers;

[Route("Authorization")]
public class Authorization : Controller
{

    private readonly JwtSettings _options;
    private string? _token;
    private readonly ILogger<Authorization> _logger;
    private readonly ApplicationContext _context;

    public Authorization(IOptions<JwtSettings> options, ILogger<Authorization> logger, ApplicationContext context)
    {
        _logger = logger;
        _options = options.Value;
        _context = context;
    }

    [HttpPost, Route("Auth")]
    public async Task<IActionResult>? Auth([FromServices] ISender sender)
    {

        try
        {
            Users? userData = await HttpContext.Request.ReadFromJsonAsync<Users>();

            Users? person = await sender.Send(new GetUserQuery(
                userData?.Firstname,
                userData?.Lastname,
                userData?.Patronymic,
                (int)userData!.GroupsId!,
                userData.Password));

            if (person is null) return Json(new { status = false, text = "Пользователь не найден" });

            string username = $"{person?.Firstname?.Replace(" ", "")} " +
                                                $"{person?.Lastname?.Replace(" ", "")}";

            TokenSecurity tokenSecurity = new(_options, username);

            _token = tokenSecurity.GetToken();

            var response = new
            {
                accessToken = _token,
                username = tokenSecurity._claimsCreator.GetClaims().Name,
                group = person?.Groups?.Name,
                groupId = person?.Groups?.Id,
                id = person?.Id
            };

            Sessions.CreateSession(person?.Id, person?.Firstname, person?.Lastname, DateTime.Now);

            return Json(response);

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
    public JsonResult GetGroups()
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