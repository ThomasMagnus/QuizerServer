using MediatR;
using Quizer.Models;

namespace QuizerServer.Requests.UsersRequests
{
    public record TeacherQuery(string? fname,
                                string? lname,
                                string? login,
                                string? password) : IRequest<Teacher>;
}
