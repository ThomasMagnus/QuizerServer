using MediatR;
using Microsoft.EntityFrameworkCore;
using Quizer.Context;
using Quizer.Models;
using QuizerServer.Requests.UsersRequests;

namespace QuizerServer.Handlers
{
    public class GetTeacherQueryHandler : IRequestHandler<TeacherQuery, Teacher>
    {
        private readonly ApplicationContext _context;
        public GetTeacherQueryHandler(ApplicationContext context)
        {
            _context = context;
        }
        public async Task<Teacher> Handle(TeacherQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Teacher> teachers = _context.Teachers;

            Teacher? teacher = await teachers.FirstOrDefaultAsync(x => x.fname!.ToLower().Trim() == request!.fname!.ToLower().Trim() &&
                                                                x.lname!.ToLower().Trim() == request.lname!.ToLower().Trim() &&
                                                                x.login!.ToLower().Trim() == request.login!.ToLower().Trim() &&
                                                                x.password == request.password, cancellationToken);

            return teacher!;
        }
    }
}
