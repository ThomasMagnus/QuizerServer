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
        public Dictionary<string, string[]> GetTeacherProps()
        {
            Dictionary<string, List<string>> teacherProps = new();

            using TeacherPropsContext teacherPropsContext = new();
            using TeacherContext teacherContext = new();
            using SubjectsContext subjectsContext = new();

            List<TeacherProps> teacherPropsList = teacherPropsContext.TeacherProps.Where(x => x.teacherid == teacherId).OrderBy(x => x.id).ToList();
            List<Subjects>? subjectsList = subjectsContext?.subjects?.ToList();
            List<Groups> groupsList = new GroupsServices() { db = new ApplicationContext() }.EntityLIst().Result;

            List<string>? subjects = new();
            List<string>? groups = new();

            Dictionary<string, string[]> keyValuePairs = new Dictionary<string, string[]>();

            foreach (TeacherProps? item in teacherPropsList)
            {
                Subjects? subject = subjectsList?.FirstOrDefault(x => x?.Id == item?.subjectsid);

                string[] groupsArray = item.groupsid!.SelectMany(x => groupsList.Where(y => x == y?.Id).Select(x => x.Name)).ToArray()!;

                keyValuePairs[subject?.Name!] = groupsArray;
            }
            return keyValuePairs;
        }

        public async void DeleteSubject(string props, int teacherId)
        {
            using TeacherPropsContext teacherPropsContext = new();
            using SubjectsContext subjectsContext = new();

            Subjects? subjects = subjectsContext?.subjects?.AsQueryable().FirstOrDefault(x => x.Name == props);
            TeacherProps? teacherProps = teacherPropsContext?.TeacherProps?.FirstOrDefault(x => x!.subjectsid == subjects!.Id && x!.teacherid == teacherId);

            if (teacherProps is not null)
            {
                teacherPropsContext?.Remove(teacherProps!);
                await teacherPropsContext?.SaveChangesAsync()!;
            }
            else Console.WriteLine("Предмет не найден!");
        }
        public async void DeleteGroup(string groupName, string subjectName, int teacherId)
        {
            using TeacherPropsContext teacherPropsContext = new();
            using SubjectsContext subjectsContext = new();
            GroupsServices groupsServices = new GroupsServices() { db = _applicationContext };

            Subjects? subjects = subjectsContext?.subjects?.AsQueryable().FirstOrDefault(x => x.Name == subjectName);
            List<Groups> groupsList = await groupsServices.EntityLIst();
            TeacherProps? teacherProps = teacherPropsContext?.TeacherProps?.FirstOrDefault(x => x!.subjectsid == subjects!.Id && x.teacherid == teacherId);

            Groups? groups = await groupsServices.GetEntity(new Dictionary<string, object> { { "name", groupName } });

            teacherProps!.groupsid = teacherProps.groupsid?.Where(x => x != groups?.Id).ToArray();

            await teacherPropsContext?.SaveChangesAsync()!;
        }
    }
}
