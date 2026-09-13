using MediatR;

namespace MiniERP.Application.Commands.Auth
{
    public record DeleteUserCommand(int AdminId, int UserId) : IRequest<bool>;
}