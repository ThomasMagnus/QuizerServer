using MediatR;
using Microsoft.EntityFrameworkCore;
using Quizer.Context;
using Quizer.Models;
using QuizerServer.Requests.UsersRequests;

namespace QuizerServer.Handlers;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, Users>
{
    private readonly ApplicationContext _context;

    public GetUserQueryHandler(ApplicationContext context)
    {
        _context = context;
    }
    public async Task<Users> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Users> users = _context.Users;

        Users? user = await users
            .Include(x => x.Groups)
            .FirstOrDefaultAsync(x => x.Firstname!.ToLower() == request.firstName!.ToString()!.ToLower()
                                        && x.Lastname!.ToLower() == request.lastName!.ToString()!.ToLower()
                                        && x.Patronymic!.ToLower() == request.patronymic!.ToString()!.ToLower()
                                        && x.Password == request.password!.ToString()
                                        && x.GroupsId == int.Parse(request.groupId.ToString()!), cancellationToken);

        return user!;
    }
}