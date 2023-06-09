using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quizer.Context;
using Quizer.Models;

namespace Quizer.Controllers
{
    public class AdminPage : Controller
    {
        private ApplicationContext _context;

        public AdminPage(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet, Route("[controller]/Index")]
        [Authorize]
        public async Task<JsonResult> Index()
        {
            List<Admin> adminList = await _context.Admin.ToListAsync();

            Console.WriteLine("Admin page");
            return Json("Admin Page");
        }
    }
}
