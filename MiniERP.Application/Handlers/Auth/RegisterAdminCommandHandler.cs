using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using MiniERP.Application.Commands.Auth;
using MiniERP.Core.Constants;
using MiniERP.Core.Entities;
using MiniERP.Core.Interfaces;

namespace MiniERP.Application.Handlers.Auth;

public class RegisterAdminCommandHandler(
    IUnitOfWork unitOfWork,
    ISecurityLogService securityLogService,
    ILogger<RegisterAdminCommandHandler> _logger)
    : IRequestHandler<RegisterAdminCommand, bool>
{
    public async Task<bool> Handle(RegisterAdminCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email?.Trim();

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(request.Password))
        {
            _logger.LogWarning("Registration failed: Empty email or password provided by admin {AdminId}", request.AdminId);
            throw new ArgumentException("Invalid data");
        }

        var existing = await unitOfWork.Users.GetByEmailAsync(email, cancellationToken);
        if (existing != null)
        {
            _logger.LogWarning("Registration failed: Email {Email} already exists", email);
            throw new InvalidOperationException("Email already registered.");
        }

        var user = new User
        {
            Name = request.Name,
            Email = email,
            PasswordHash = PasswordHasher.Hash(request.Password),
            Role = Roles.Admin,
            CreatedAt = DateTime.UtcNow
        };

        await unitOfWork.Users.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await securityLogService.LogAsync(
            actorUserId: request.AdminId,
            targetUserId: user.Id,
            action: "REGISTER_ADMIN",
            description: $"Admin {request.AdminId} registered new admin: {user.Email}"
        );

        _logger.LogInformation("Successfully registered new admin: {Email} by AdminId: {AdminId}", email, request.AdminId);
        return true;
    }
}