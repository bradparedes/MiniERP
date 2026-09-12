using MediatR;

namespace MiniERP.Application.Commands.Auth
{
    public record RegisterAdminCommand(
        string Name,
        string Email,
        string Password,
        int AdminId) : IRequest<bool>;
}
