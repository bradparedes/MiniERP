using System;
using System.Threading.Tasks;
using MiniERP.Core.Constants;
using MiniERP.Core.Entities;
using MiniERP.Core.Interfaces;
using MiniERP.Application.Requests;

namespace MiniERP.Application.UseCases.Auth;

public class RegisterAdminUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISecurityLogService _securityLogService;

    public RegisterAdminUseCase(IUnitOfWork unitOfWork, ISecurityLogService securityLogService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _securityLogService = securityLogService ?? throw new ArgumentNullException(nameof(securityLogService));
    }

    public async Task Execute(RegisterRequest request, int adminId)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var email = request.Email?.Trim();
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(request.Email));

        var exists = await _unitOfWork.Users.GetByEmailAsync(email);
        if (exists != null)
            throw new InvalidOperationException("Email already registered.");

        var user = new User
        {
            Name = request.Name,
            Email = email,
            PasswordHash = PasswordHasher.Hash(request.Password),
            Role = Roles.Admin,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        await _securityLogService.LogAsync(
            actorUserId: adminId,
            targetUserId: user.Id,
            action: "REGISTER_ADMIN",
            description: $"Admin {adminId} registered new admin: {user.Email}"
        );
    }
}