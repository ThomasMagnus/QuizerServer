using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quizer.Models;

namespace Quizer.Controllers
{
    public class AdminPage : Controller
    {
        [HttpGet, Route("[controller]/Index")]
        [Authorize]
        public JsonResult Index()
        {
            AdminsContext adminContext = new();
            List<Admin> adminList = adminContext.Admin.ToList();

            Console.WriteLine("Admin page");
            return Json("Admin Page");
        }
    }
}
