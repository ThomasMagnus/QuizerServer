using MediatR;
using Quizer.Models;

namespace QuizerServer.Requests.UsersRequests;

public record GetUserQuery(string? firstName,
                            string? lastName,
                            string? patronymic,
                            int groupId,
                            string? password) : IRequest<Users>;
