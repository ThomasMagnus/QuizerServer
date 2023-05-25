using Quizer.Context;
using Quizer.IServices;
using Quizer.Models;

namespace Quizer.Services
{
    public class SubjectsProps : ISubjectsProps
    {
        public List<Subjects> GetSubjectsList() {
            using SubjectsContext subjectsContext = new();

            return subjectsContext?.subjects?.ToList()!;
        }
    }
}
