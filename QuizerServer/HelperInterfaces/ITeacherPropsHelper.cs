using Quizer.Models;

namespace QuizerServer.HelperInterfaces
{
    public interface ITeacherPropsHelper
    {
        public Task<Dictionary<string, string[]>> GetTeacherProps();
        public Task DeleteSubject(string props, int teacherId);
        public Task DeleteGroup(string groupName, string subjectName, int teacherId);
    }
}
