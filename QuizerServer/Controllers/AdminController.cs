using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Quizer.Context;
using Quizer.Models;
using QuizerServer.HelperClasses;
using System.Text.Json;

namespace Quizer.Controllers
{
    public class AdminController : Controller
    {
        private readonly JwtSettings _options;
        private ApplicationContext _context;
        public AdminController(IOptions<JwtSettings> options, ApplicationContext context) => (_options, _context) = (options.Value, context);

        [HttpPost, Route("[controller]/Index")]
        public async Task<IActionResult> Index([FromBody] JsonElement value)
        {
            Dictionary<string, string>? data = JsonSerializer.Deserialize<Dictionary<string, string>>(value);

            Admin? admin = await _context.Admin.FirstOrDefaultAsync(x => x.FName!.ToLower().Replace(" ", "") == data["firstname"].Trim() 
                                                    && x.LName!.ToLower().Replace(" ", "") == data["lastname"].Trim()
                                                    && x.Login!.ToLower().Replace(" ", "") == data["login"].Trim()
                                                    && x.Password!.Replace(" ", "") == data["password"].Replace(" ", ""));

            if (admin is null) { return Json("Администратор не найден"); }
            string username = string.Format("{0} {1}", admin?.FName?.Replace(" ", ""), admin?.LName?.Replace(" ", ""));
            TokenSecurity tokenSecurity = new(_options, username);
            string tokenHandler = tokenSecurity.GetToken();

            var response = new
            {
                accessAdminToken = tokenHandler,
                username = tokenSecurity._claimsCreator.GetClaims().Name,
                admin?.id

            };

            return Json(response);
        }
    }
}
