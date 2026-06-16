using MiniERP.Core.Entities;
using MiniERP.Core.Interfaces;
using MiniERP.Core.Constants;
using MiniERP.Application.DTOs.Auth;
using MiniERP.Application.Exceptions;
namespace MiniERP.Application.UseCases.Auth;

public class RegisterUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ISecurityLogService _securityLogService;

    public RegisterUseCase(
        IUserRepository userRepository,
        ISecurityLogService securityLogService)
    {
        _userRepository = userRepository;
        _securityLogService = securityLogService;
    }

    public async Task Execute(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            throw new BadRequestException("Email and password are required");

        var email = request.Email.Trim().ToLower();

        var existingUser = await _userRepository.GetByEmail(email);

        if (existingUser != null)
            throw new BadRequestException("Email is already registered");

        var user = new User
        {
            Email = email,
            PasswordHash = PasswordHasher.Hash(request.Password),
            Role = Roles.User
        };

        await _userRepository.Add(user);

        await _securityLogService.LogAsync(
            actorUserId: null,
            targetUserId: user.Id,
            action: "REGISTER_USER",
            description: "User registered successfully."
        );
    }
}