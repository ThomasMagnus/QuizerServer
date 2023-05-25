using Quizer.Models;

namespace QuizerServer.HelperInterfaces
{
    public interface ITeacherPropsHelper
    {
        public Dictionary<string, string[]> GetTeacherProps();
        public void DeleteSubject(string props, int teacherId);
        public void DeleteGroup(string groupName, string subjectName, int teacherId);
    }
}
