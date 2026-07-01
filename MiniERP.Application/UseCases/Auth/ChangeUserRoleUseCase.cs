using MiniERP.Core.Interfaces;
using MiniERP.Core.Constants;
using MiniERP.Application.DTOs.Auth;
using MiniERP.Application.Exceptions;

namespace MiniERP.Application.UseCases.Auth;

public class ChangeUserRoleUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ISecurityLogService _securityLogService;

    public ChangeUserRoleUseCase(
        IUserRepository userRepository,
        ISecurityLogService securityLogService)
    {
        _userRepository = userRepository;
        _securityLogService = securityLogService;
    }

    public async Task Execute(ChangeUserRoleRequest request, int adminId)
    {
        if (request.UserId <= 0 || string.IsNullOrWhiteSpace(request.NewRole))
            throw new BadRequestException("Invalid data");

        if (request.NewRole != Roles.Admin && request.NewRole != Roles.User)
            throw new BadRequestException("Invalid role");

        var user = await _userRepository.GetById(request.UserId);

        if (user == null)
            throw new NotFoundException("User not found");

        user.Role = request.NewRole;
        await _userRepository.Update(user);

        await _securityLogService.LogAsync(
            actorUserId: adminId,
            targetUserId: user.Id,
            action: "CHANGE_ROLE",
            description: $"Role changed to {request.NewRole}"
        );
    }
}