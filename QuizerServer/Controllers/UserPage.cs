using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Quizer.Context;
using Quizer.Models;

namespace Quizer.Controllers
{
    public class UserPage : Controller
    {
        private readonly IMemoryCache _cache;
        private readonly ApplicationContext _context;
        public UserPage(IMemoryCache cache, ApplicationContext context)
        {
            _cache = cache;
            _context = context;
        }

        [HttpGet, Route("[controller]/Index")]
        [Authorize]
        public void Index()
        {
            RedirectToAction("UserData", "UserPage");
        }
        public IActionResult UserData()
        {
            return Json("Hello, user!");
        }

        [HttpGet]
        [Route("[controller]/GetSubjects")]
        public async Task<IActionResult> GetSubjects()
        {
            _cache.TryGetValue(123, out List<Subjects>? subjects);

            if (subjects is null)
            {
                try
                {
                    subjects = await _context.Subjects.ToListAsync();
                    _cache.Set(123, subjects);
                    return Json(subjects);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return Json("Ошибка запроса");
                }
            }
            return Json(subjects);
        }

        [HttpGet]
        public IActionResult GetTasks(int subjectId, int groupId)
        {
            _cache.TryGetValue(subjectId, out List<Tasks>? tasks);

            if (tasks is null)
            {
                try
                {
                    tasks = _context.Tasks
                                        .Where(x => x.subjectid == subjectId && x.groupid == groupId)
                                        .OrderByDescending(x => x.Putdate).ToList();

                    _cache.Set(subjectId, tasks, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)));

                    return Json(tasks);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    return Json("Произошла ошибка в запросе Tasks");
                }
            }

            return Json(tasks);
        }

    }
}
