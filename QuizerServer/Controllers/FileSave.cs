using Microsoft.AspNetCore.Mvc;
using Quizer.Context;
using Quizer.Models;
using Quizer.RequestBody;
using Quizer.Context;
using Microsoft.EntityFrameworkCore;

namespace Quizer.Controllers
{

    [Route("fileSave")]
    public class FileSave : Controller
    {
        private ApplicationContext _context;
        IConfiguration? _configuration;
        private readonly ILogger<FileSave> _logger;

        public FileSave(IConfiguration? configuration, ILogger<FileSave> logger, ApplicationContext context)
        {
            _configuration = configuration;
            _logger = logger;
            _context = context;
        }

        [HttpPost("save")]
        public async Task<IActionResult> Index([FromForm] FileModel formFile)
        {
            Console.WriteLine(formFile.GroupName);
            Console.WriteLine(formFile.SubjectName);
            try
            {
                //string? fileDirectory = _configuration?.GetSection("FilePath").Value;
                string? fileDirectory = Directory.GetCurrentDirectory() + "Files";
                if (formFile is not null)
                {
                    string file = fileDirectory + formFile?.File?.FileName;

                    //List<Groups> groups = await _context.Groups.ToListAsync();
                    //List<Subjects> subjects = await _context.Subjects.ToListAsync();

                    Subjects? subject = await _context.Subjects
                        .FirstOrDefaultAsync(x => x.Name!.ToLower() == formFile!.SubjectName!.ToLower());
                    Groups? group = await _context.Groups
                        .FirstOrDefaultAsync(x => x.Name!.ToLower() == formFile!.GroupName!.ToLower());

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

                    await _context.AddAsync(tasks);
                    await _context.SaveChangesAsync();

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
