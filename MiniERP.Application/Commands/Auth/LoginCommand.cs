using MediatR;

namespace MiniERP.Application.Commands.Auth
{
    public record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;
}

// DTO de respuesta
public record LoginResponse(string Token, int UserId, string Email, string Role);