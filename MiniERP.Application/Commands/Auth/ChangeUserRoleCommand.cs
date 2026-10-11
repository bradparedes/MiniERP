using MediatR;

namespace MiniERP.Application.Commands.Auth;

public record ChangeUserRoleCommand(int UserId, string NewRole, int AdminId) : IRequest<bool>;