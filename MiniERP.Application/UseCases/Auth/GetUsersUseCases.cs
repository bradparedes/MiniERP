using MiniERP.Core.Interfaces;
using MiniERP.Application.DTOs.Users;

namespace MiniERP.Application.UseCases.Auth;

public class GetUsersUseCase
{
    private readonly IUserRepository _userRepository;

    public GetUsersUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<UserResponse>> Execute()
    {
        var users = await _userRepository.GetAll();
        return users.Select(u => new UserResponse
        {
            Id = u.Id,
            Email = u.Email,
            Role = u.Role,
            CreatedAt = u.CreatedAt
        }).ToList();
    }
}