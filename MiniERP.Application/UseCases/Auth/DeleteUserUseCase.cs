using MiniERP.Core.Constants;
using MiniERP.Core.Interfaces;
using MiniERP.Application.Exceptions;

namespace MiniERP.Application.UseCases.Auth;

public class DeleteUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ISecurityLogService _securityLogService;

    public DeleteUserUseCase(
        IUserRepository userRepository,
        ISecurityLogService securityLogService)
    {
        _userRepository = userRepository;
        _securityLogService = securityLogService;
    }

    public async Task Execute(int adminId, int userId)
    {
        var user = await _userRepository.GetById(userId);

        if (user == null)
            throw new NotFoundException("User not found");

        // No permitir eliminar al último administrador
        if (user.Role == Roles.Admin)
        {
            var adminCount = await _userRepository.CountAdmins();

            if (adminCount <= 1)
                throw new BadRequestException("Cannot delete the last administrator");
        }

        await _userRepository.Delete(user);

        await _securityLogService.LogAsync(
            actorUserId: adminId,
            targetUserId: user.Id,
            action: "DELETE_USER",
            description: "User deleted"
        );
    }
}