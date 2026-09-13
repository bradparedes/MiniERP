using MediatR;

namespace MiniERP.Application.Commands.Auth
{
    public record RegisterCommand(string Name, string Email, string Password) : IRequest<bool>;
}