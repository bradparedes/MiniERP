using MiniERP.Application.DTOs.Auth;
using MiniERP.Application.Exceptions;
using MiniERP.Core.Entities;
using MiniERP.Core.Interfaces;

namespace MiniERP.Application.UseCases.Auth;

public class ChangePasswordUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ISecurityLogService _securityLogService;

    public ChangePasswordUseCase(
        IUserRepository userRepository,
        ISecurityLogService securityLogService)
    {
        _userRepository = userRepository;
        _securityLogService = securityLogService;
    }

    public async Task Execute(ChangePasswordRequest request, int userId)
    {
        if (string.IsNullOrWhiteSpace(request.CurrentPassword) ||
            string.IsNullOrWhiteSpace(request.NewPassword))
        {
            throw new BadRequestException(
                "Current password and new password are required.");
        }

        var user = await _userRepository.GetById(userId);

        if (user == null)
            throw new UnauthorizedException("User not found.");

        if (!PasswordHasher.Verify(request.CurrentPassword, user.PasswordHash))
            throw new BadRequestException("Current password is incorrect.");

        user.PasswordHash = PasswordHasher.Hash(request.NewPassword);

        await _userRepository.Update(user);

        await _securityLogService.LogAsync(
            actorUserId: user.Id,
            targetUserId: user.Id,
            action: "CHANGE_PASSWORD",
            description: "User changed their password."
        );
    }
}