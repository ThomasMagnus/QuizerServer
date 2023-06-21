using Microsoft.EntityFrameworkCore;
using Quizer.Context;
using Quizer.Models;
using QuizerServer.HelperInterfaces;

namespace QuizerServer.HelperClasses
{
    public class TeacherPropsHelper : ITeacherPropsHelper
    {
        public int teacherId { get; set; }
        private ApplicationContext? _applicationContext { get; set; }

        public ApplicationContext ContextName
        {
            set => _applicationContext = value;
        }
        public async Task<Dictionary<string, string[]>> GetTeacherProps()
        {
            Dictionary<string, List<string>> teacherProps = new();
            using TeacherContext teacherContext = new();

            List<TeacherProps>? teacherPropsList = await _applicationContext!.TeacherProps
                                                                            .Include(x => x.Subjects)
                                                                            .Where(x => x.teacherid == teacherId).OrderBy(x => x.id)
                                                                            .ToListAsync();

            List<Groups> groupsList = new GroupsServices() { db = _applicationContext }.EntityLIst().Result;
            List<string>? groups = new();

            Dictionary<string, string[]> keyValuePairs = new Dictionary<string, string[]>();

            foreach (TeacherProps? item in teacherPropsList)
            {
                string[] groupsArray = item.groupsid!
                                            .SelectMany(x => groupsList
                                                            .Where(y => x == y?.Id)
                                                            .Select(x => x.Name))
                                            .ToArray()!;
                keyValuePairs[item.Subjects?.Name!] = groupsArray;
            }
            return keyValuePairs;
        }

        public async Task DeleteSubject(string props, int teacherId)
        {
            Subjects? subjects = _applicationContext?.Subjects?.AsQueryable().FirstOrDefault(x => x.Name == props);
            
            TeacherProps? teacherProps = await _applicationContext!.TeacherProps
                                                                   .Include(x => x.Subjects)               
                                                                   .FirstOrDefaultAsync(
                                                                        x => x!.subjectsid == subjects!.Id && x!.teacherid == teacherId);

            if (teacherProps is not null)
            {
                _applicationContext?.Remove(teacherProps!);
                await _applicationContext?.SaveChangesAsync()!;
            }
            else Console.WriteLine("Предмет не найден!");
        }
        public async Task DeleteGroup(string groupName, string subjectName, int teacherId)
        {
            GroupsServices groupsServices = new GroupsServices() { db = _applicationContext };

            Subjects? subjects = await _applicationContext!.Subjects
                                                     .AsQueryable()
                                                     .FirstOrDefaultAsync(x => x.Name == subjectName);

            List<Groups> groupsList = await groupsServices.EntityLIst();
            TeacherProps? teacherProps = await _applicationContext!.TeacherProps
                                                             .FirstOrDefaultAsync(x => x!.subjectsid == subjects!.Id && x.teacherid == teacherId);

            Groups? groups = await groupsServices.GetEntity(new Dictionary<string, object> { { "name", groupName } });

            teacherProps!.groupsid = teacherProps.groupsid?
                                                 .Where(x => x != groups?.Id)
                                                 .ToArray();

            await _applicationContext?.SaveChangesAsync()!;
        }
    }
}
