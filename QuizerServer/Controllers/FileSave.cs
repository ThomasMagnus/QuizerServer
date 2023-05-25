using Microsoft.AspNetCore.Mvc;
using Quizer.Context;
using Quizer.Models;
using Quizer.RequestBody;

namespace Quizer.Controllers
{
    [Route("fileSave")]
    public class FileSave : Controller
    {
        IConfiguration? _configuration;
        private readonly ILogger<FileSave> _logger;

        public FileSave(IConfiguration? configuration, ILogger<FileSave> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("save")]
        public async Task<IActionResult> Index([FromForm] FileModel formFile)
        {
            Console.WriteLine(formFile.GroupName);
            Console.WriteLine(formFile.SubjectName);
            try
            {
                string? fileDirectory = _configuration?.GetSection("FilePath").Value;
                if (formFile is not null)
                {
                    string file = fileDirectory + formFile?.File?.FileName;

                    using ApplicationContext applicationContext = new();

                    List<Groups> groups = applicationContext.Groups.ToList();
                    List<Subjects> subjects = applicationContext.Subjects.ToList();

                    Subjects? subject = subjects
                        .FirstOrDefault(x => x.Name?.ToLower() == formFile?.SubjectName?.ToLower());
                    Groups? group = groups
                        .FirstOrDefault(x => x.Name?.ToLower() == formFile?.GroupName?.ToLower());

                    if (group is null) return Json(new
                    {
                        StatusCode = 404,
                        Message = $"Группа: {formFile?.GroupName} в базе не найдена!"
                    });
                    else if(subject is null) return Json(new
                    {
                        StatusCode = 404,
                        Message = $"Предмет: {formFile?.SubjectName} в базе не найден!"
                    });

                    Tasks tasks = new Tasks
                    {
                        Filepath = file,
                        Filename = formFile?.File?.FileName,
                        subjectid = subject?.Id,
                        groupid = group?.Id,
                        Putdate = DateTime.UtcNow
                    };

                    await applicationContext.AddAsync(tasks);
                    await applicationContext.SaveChangesAsync();

                    using (FileStream fileStream = new FileStream(file, FileMode.Create))
                    {
                        await formFile?.File?.CopyToAsync(fileStream)!;
                    }
                }

                return Json(new
                {
                    statusCode = 200,
                    message = "Файл отправлен!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return Json(new
                {
                    statusCode = 500,
                    message = "Ошибка на сервере!"
                });
            }
        }
    }
}
