using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MiniERP.Application.Commands.Auth;
using MiniERP.Core.Interfaces;
using MiniERP.Core.Entities; // User + PasswordHasher
using MiniERP.Core.Constants;
using Microsoft.Extensions.Logging;

namespace MiniERP.Application.Handlers.Auth;

public class RegisterCommandHandler(
    IUnitOfWork unitOfWork,
    ISecurityLogService securityLogService,
    ILogger<RegisterAdminCommandHandler> _logger) 
    : IRequestHandler<RegisterCommand, bool>
{
    public async Task<bool> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email?.Trim();

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(request.Password))
            throw new ArgumentException("Invalid data");

        var existing = await unitOfWork.Users.GetByEmailAsync(email, cancellationToken);
        if (existing != null)
            throw new InvalidOperationException("Email already registered.");

        var user = new User
        {
            Name = request.Name,
            Email = email,
            PasswordHash = PasswordHasher.Hash(request.Password),
            Role = Roles.User,
            CreatedAt = DateTime.UtcNow
        };

        await unitOfWork.Users.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken); // ✅ Token incluido

        await securityLogService.LogAsync(
            0,
            user.Id,
            "REGISTER_USER",
            $"New user registered: {user.Email}"
        );

        _logger.LogInformation("Admin registered successfully: {Email}", email);

        return true;
    }
}