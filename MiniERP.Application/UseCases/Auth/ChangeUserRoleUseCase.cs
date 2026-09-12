using System;
using System.Threading.Tasks;
using MiniERP.Core.Interfaces;
using MiniERP.Core.Constants;
using MiniERP.Core.Entities;
using MiniERP.Application.DTOs.Auth;
using MiniERP.Application.Exceptions;

namespace MiniERP.Application.UseCases.Auth;

public class ChangeUserRoleUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISecurityLogService _securityLogService;

    public ChangeUserRoleUseCase(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ISecurityLogService securityLogService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _securityLogService = securityLogService ?? throw new ArgumentNullException(nameof(securityLogService));
    }

    public async Task Execute(ChangeUserRoleRequest request, int adminId)
    {
        if (request.UserId <= 0 || string.IsNullOrWhiteSpace(request.NewRole))
            throw new BadRequestException("Invalid data");

        if (request.NewRole != Roles.Admin && request.NewRole != Roles.User)
            throw new BadRequestException("Invalid role");

        var user = await _unitOfWork.Users.GetById(request.UserId);

        if (user == null)
            throw new NotFoundException("User not found");

        user.Role = request.NewRole;

        await _unitOfWork.SaveChangesAsync();

        await _securityLogService.LogAsync(
            actorUserId: adminId,
            targetUserId: user.Id,
            action: "CHANGE_ROLE",
            description: $"Role changed to {request.NewRole}"
        );
    }
}

