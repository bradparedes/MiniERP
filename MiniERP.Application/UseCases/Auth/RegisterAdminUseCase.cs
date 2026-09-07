using MiniERP.Core.Interfaces;
using MiniERP.Core.Entities;
using MiniERP.Core.Constants;
using MiniERP.Application.Exceptions;
namespace MiniERP.Application.UseCases.Auth;

public class RegisterAdminUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ISecurityLogService _securityLogService;

    public RegisterAdminUseCase(
        IUserRepository userRepository,
        ISecurityLogService securityLogService)
    {
        _userRepository = userRepository;
        _securityLogService = securityLogService;
    }

    public async Task Execute(
        RegisterRequest request,
        int adminId)
    {
        var user = new User
        {
            Email = request.Email,
            PasswordHash = request.Password,
            Role = Roles.Admin,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.Add(user);
    }
}
