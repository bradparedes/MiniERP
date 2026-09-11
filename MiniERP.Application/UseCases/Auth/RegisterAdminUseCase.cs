using System;
using System.Threading.Tasks;
using MiniERP.Core.Interfaces;
using MiniERP.Core.Entities;
using MiniERP.Core.Constants;
using MiniERP.Application.Exceptions;
namespace MiniERP.Application.UseCases.Auth;

public class RegisterAdminUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly ISecurityLogService _securityLogService;

    public RegisterAdminUseCase(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        ISecurityLogService securityLogService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _securityLogService = securityLogService ?? throw new ArgumentNullException(nameof(securityLogService));
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
        await _unitOfWork.Users.Add(user);

        await _unitOfWork.SaveChangesAsync();

        await _securityLogService.LogAsync(
            adminId,
            user.Id,
            "RegisterAdmin",
            $"Admin {adminId} registered new admin user: {user.Email}"
        );
        await _userRepository.Add(user);
    }
}

