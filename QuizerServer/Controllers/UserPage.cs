using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Quizer.Context;
using Quizer.Models;
using System.Text.Json;

namespace Quizer.Controllers
{
    public class UserPage : Controller
    {
        IMemoryCache _cache;

        public UserPage(IMemoryCache cache)
        {
            _cache = cache;
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
        public IActionResult GetSubjects()
        {
            using SubjectsContext subjectsContext = new();
            try
            {
                List<Subjects>? subjects = subjectsContext?.subjects?.ToList();
                return Json(subjects);
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return Json("Ошибка запроса");
            }
        }

        [HttpPost]
        public IActionResult GetUserProps([FromBody] JsonElement value)
        {
            using (SessionsContext sessionContext = new())
            using (ApplicationContext applicationContext = new())

            try
            {
                List<Sessions>? sessions = sessionContext?.Sessions?.ToList();
                Sessions? session = sessions?.FirstOrDefault(x => x.Id == 3);
                GroupsServices groupsServices = new() { db = applicationContext };

                string? groupName = session?.User?.Groups?.Name;
                Console.WriteLine(groupName);

                    var userProperty = new
                {
                    firstname = session?.UserFirstname,
                    lastname = session?.UserLastname,
                    group = groupName,
                    id = session?.Id,
                };

                return Json(userProperty);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(ex);
            }
        }

        [HttpGet]
        public IActionResult GetTasks(int subjectId, int groupId)
        {
            _cache.TryGetValue(subjectId, out Tasks? task);

            if (task is null)
            {
                try
                {
                    using ApplicationContext applicationContext = new();
                    List<Tasks>? tasks = applicationContext.Tasks.ToList()
                        .Where(x => x.subjectid == subjectId && x.groupid == groupId)
                        .OrderByDescending(x => x?.Putdate).ToList();

                    _cache.Set(subjectId, tasks, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)));

                    return Json(tasks);
                    //return Json("Запрос успешно прошёл");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка в Tasks");
                    Console.WriteLine(ex.ToString());
                    return Json("Произошла ошибка в запросе Tasks");
                }
            }

            return Json(task);
        }

    }
}
