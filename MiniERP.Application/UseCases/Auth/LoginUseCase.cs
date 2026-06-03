using MiniERP.Core.Interfaces;
using MiniERP.Application.Requests;
using MiniERP.Core.Entities;
using MiniERP.Application.Interfaces;
using MiniERP.Application.DTOs.Auth;
using MiniERP.Core.Constants;

namespace MiniERP.Application.UseCases.Auth;
public class LoginUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ISecurityLogService _securityLogService;

    public LoginUseCase(
        IUserRepository userRepository,
        ISecurityLogService securityLogService)
    {
        _userRepository = userRepository;
        _securityLogService = securityLogService;
    }

    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message) { }
    }
    public async Task<LoginResponse> Execute(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            throw new Exception("Email y contraseña son obligatorios");

        var email = request.Email.Trim().ToLower();

        var user = await _userRepository.GetByEmail(email);

        if (user == null || !PasswordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Credenciales incorrectas");

        await _securityLogService.LogAsync(
            actorUserId: user.Id,
            targetUserId: user.Id,
            action: "LOGIN",
            description: "Inicio de sesión exitoso."
        );

        return new LoginResponse
        {
            UserId = user.Id,
            Email = user.Email,
            Role = user.Role 
        };
    }
}