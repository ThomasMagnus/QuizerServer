using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Quizer.Models;
using Microsoft.Extensions.Options;
using System.Text;
using QuizerServer.HelperClasses;

namespace Quizer.Controllers
{
    public class AdminController : Controller
    {
        private readonly JwtSettings _options;
        public AdminController(IOptions<JwtSettings> options)
        {
            _options = options.Value;
        }

        [HttpPost, Route("[controller]/Index")]
        public IActionResult Index([FromBody] JsonElement value)
        {
            Dictionary<string, string>? data = JsonSerializer.Deserialize<Dictionary<string, string>>(value);

            using AdminsContext adminContext = new();

            List<Admin> adminsList = adminContext.Admin.ToList();

            Admin? admin = adminsList.FirstOrDefault(x => x.FName?.ToLower().Replace(" ", "") == data?["firstname"].Trim() 
                                                    && x.LName?.ToLower().Replace(" ", "") == data?["lastname"].Trim()
                                                    && x.Login?.ToLower().Replace(" ", "") == data?["login"].Trim()
                                                    && x.Password?.Replace(" ", "") == data?["password"].Replace(" ", ""));

            if (admin is null) { return Json("Администратор не найден"); }
            string username = String.Format("{0} {1}", admin?.FName?.Replace(" ", ""), admin?.LName?.Replace(" ", ""));
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
