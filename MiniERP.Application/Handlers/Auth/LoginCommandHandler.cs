using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MiniERP.Application.Commands.Auth;
using MiniERP.Core.Interfaces;
using MiniERP.Core.Entities;
using Microsoft.Extensions.Logging;

namespace MiniERP.Application.Handlers.Auth;

public class LoginCommandHandler(
    IUnitOfWork unitOfWork,
    ITokenService tokenService,
    ISecurityLogService securityLogService,
    ILogger<LoginCommandHandler> logger) 
    : IRequestHandler<LoginCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // 1. Normalización segura: Solo Trim en Email
        var email = request.Email?.Trim();

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(request.Password))
        {
            logger.LogWarning("Login attempt with empty email or password.");
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        // 2. Búsqueda case-insensitive optimizada (EF.Functions.Like) con CancellationToken
        var user = await unitOfWork.Users.GetByEmailAsync(email, cancellationToken);

        if (user == null)
        {
            logger.LogWarning("Login failed: User not found for email {Email}", email);
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        // 3. Validación defensiva: El hash DEBE existir en la entidad rastreada
        if (string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            logger.LogError("Critical: PasswordHash is null or empty in DB for UserId {UserId}. Check mapping/seeding.", user.Id);
            throw new InvalidOperationException("Authentication configuration error.");
        }

        // 4. Verificación criptográfica CORRECTA con BCrypt (Extrae Salt del hash almacenado)
        bool isValid = PasswordHasher.Verify(request.Password, user.PasswordHash);

        if (!isValid)
        {
            logger.LogWarning("Login failed: Invalid password for user {Email}", email);
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        // 5. Generar Token JWT
        var token = tokenService.GenerateToken(user);

        // 6. Auditoría de éxito
        await securityLogService.LogAsync(
            actorUserId: user.Id,
            targetUserId: user.Id,
            action: "LOGIN_SUCCESS",
            description: $"User {user.Email} logged in successfully."
        );

        // 7. Respuesta
        return new LoginResponse(
            Token: token,
            UserId: user.Id,
            Email: user.Email,
            Role: user.Role
        );
    }
}