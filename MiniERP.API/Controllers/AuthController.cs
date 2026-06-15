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
        private readonly ISecurityLogService _securityLogService;
        private readonly ITokenService _tokenService;
        private readonly ChangeUserRoleUseCase _changeUserRoleUseCase;

        public AuthController(
            RegisterUseCase registerUseCase,
            LoginUseCase loginUseCase,
            IUserRepository userRepository,
            ISecurityLogService securityLogService,
            ITokenService tokenService,
            ChangeUserRoleUseCase changeUserRoleUseCase)
        {
            _registerUseCase = registerUseCase;
            _loginUseCase = loginUseCase;
            _userRepository = userRepository;
            _securityLogService = securityLogService;
            _tokenService = tokenService;
            _changeUserRoleUseCase = changeUserRoleUseCase;
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
                    message = "Inicio de sesión exitoso",
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
                message = "Registro exitoso"
            });
        }

        // -------------------------
        // CAMBIAR ROL (SOLO ADMIN)
        // -------------------------
        [Authorize(Roles = $"{Roles.Admin}")]
        [HttpPut("Change-Role")]
        public async Task<IActionResult> ChangeUserRole([FromBody] ChangeUserRoleRequest request)
        {
            var userIdClaim = User.FindFirst("id")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            int adminId = int.Parse(userIdClaim);

            await _changeUserRoleUseCase.Execute(request, adminId);

            return Ok(new { message = "Rol actualizado correctamente" });
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
                message = "Lista de usuarios",
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
        [Authorize(Roles = $"{Roles.Admin}")]
        [HttpDelete("Users/{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userRepository.GetById(id);

            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            if (user.Role == Roles.Admin)
            {
                var adminCount = await _userRepository.CountAdmins();
                if (adminCount <= 1)
                    return BadRequest(new { message = "No se puede eliminar al último administrador" });
            }

            await _userRepository.Delete(user);

            var adminId = int.Parse(User.FindFirst("id")!.Value);

            await _securityLogService.LogAsync(
                actorUserId: adminId,
                targetUserId: user.Id,
                action: "DELETE_USER",
                description: "Usuario eliminado"
            );

            return Ok(new { message = "Usuario eliminado correctamente" });
        }
    }
}