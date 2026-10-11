using MediatR;

namespace MiniERP.Application.Commands.Auth;

public record ChangePasswordCommand(string CurrentPassword, string NewPassword, int UserId) : IRequest<bool>;