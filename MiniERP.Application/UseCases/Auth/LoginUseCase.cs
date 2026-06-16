using MiniERP.Core.Interfaces;
using MiniERP.Application.Requests;
using MiniERP.Core.Entities;
using MiniERP.Application.Interfaces;
using MiniERP.Application.DTOs.Auth;
using MiniERP.Application.Exceptions;
using MiniERP.Core.Constants;

namespace MiniERP.Application.UseCases.Auth;
public class LoginUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ISecurityLogService _securityLogService;
    private readonly ITokenService _tokenService;

    public LoginUseCase(
        IUserRepository userRepository,
        ISecurityLogService securityLogService,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _securityLogService = securityLogService;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse> Execute(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            throw new BadRequestException("Email and password are required");

        var email = request.Email.Trim().ToLower();

        var user = await _userRepository.GetByEmail(email);

        var token = _tokenService.GenerateToken(user!);

        if (user == null || !PasswordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid credentials");

        await _securityLogService.LogAsync(
            actorUserId: user.Id,
            targetUserId: user.Id,
            action: "LOGIN",
            description: "Successful login."
        );

        return new LoginResponse
        {
            UserId = user.Id,
            Email = user.Email,
            Token = token,
            Role = user.Role
        };
    }
}