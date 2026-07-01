using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniERP.Core.Entities;
using MiniERP.Core.Interfaces;
using MiniERP.Core.Constants;
using MiniERP.Application.DTOs.Auth;
using MiniERP.Application.Requests;
using MiniERP.Application.UseCases.Auth;

namespace MiniERP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly RegisterUseCase _registerUseCase;
        private readonly LoginUseCase _loginUseCase;
        private readonly IUserRepository _userRepository;
        private readonly ChangeUserRoleUseCase _changeUserRoleUseCase;
        private readonly DeleteUserUseCase _deleteUserUseCase;
        public AuthController(
            RegisterUseCase registerUseCase,
            LoginUseCase loginUseCase,
            IUserRepository userRepository,
            ChangeUserRoleUseCase changeUserRoleUseCase,
            DeleteUserUseCase deleteUserUseCase)
        {
            _registerUseCase = registerUseCase;
            _loginUseCase = loginUseCase;
            _userRepository = userRepository;
            _changeUserRoleUseCase = changeUserRoleUseCase;
            _deleteUserUseCase = deleteUserUseCase;
        }

        // -------------------------
        // LOGIN
        // -------------------------
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _loginUseCase.Execute(request);

            return Ok(new
            {
            message = "Successfully logged in",
            token = result.Token,
            user = new
                {
                    id = result.UserId,
                    email = result.Email,
                    role = result.Role
                }
            });
        }

        // -------------------------
        // REGISTER
        // -------------------------
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            await _registerUseCase.Execute(request);

            return Ok(new
            {
                message = "Successfully registered"
            });
        }

        // -------------------------
        // CAMBIAR ROL (SOLO ADMIN)
        // -------------------------
        [Authorize(Roles = Roles.Admin)]
        [HttpPut("Change-Role")]
        public async Task<IActionResult> ChangeUserRole([FromBody] ChangeUserRoleRequest request)
        {
            var userIdClaim = User.FindFirst("id")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            int adminId = int.Parse(userIdClaim);

            await _changeUserRoleUseCase.Execute(request, adminId);

            return Ok(new
            {
                message = "Successfully updated role"
            });
        }

        // -------------------------
        // LISTAR USUARIOS
        // -------------------------
        [Authorize(Roles = $"{Roles.Admin}")]
        [HttpGet("Users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userRepository.GetAll();

            return Ok(new
            {
                message = "Users list",
                data = users.Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.Role,
                    u.CreatedAt
                })
            });
        }

        // -------------------------
        // ELIMINAR USUARIO
        // -------------------------
        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("Users/{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            int adminId = int.Parse(User.FindFirst("id")!.Value);

            await _deleteUserUseCase.Execute(adminId, id);

            return Ok(new
            {
                message = "User deleted successfully"
            });
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var userId = User.FindFirst("id")?.Value;
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            return Ok(new
            {
                id = userId,
                email,
                role
            });
        }
    }
}