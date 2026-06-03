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
        private readonly LoginUseCase _loginUseCase;
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly ISecurityLogService _securityLogService;

        public AuthController(
            LoginUseCase loginUseCase,
            IUserRepository userRepository,
            ITokenService tokenService,
            ISecurityLogService securityLogService)
        {
            _loginUseCase = loginUseCase;
            _userRepository = userRepository;
            _tokenService = tokenService;
            _securityLogService = securityLogService;
        }

        // -------------------------
        // LOGIN
        // -------------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
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
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        // -------------------------
        // REGISTER
        // -------------------------
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { message = "Email y contraseña son obligatorios" });

            var email = request.Email.Trim().ToLower();

            var existingUser = await _userRepository.GetByEmail(email);

            if (existingUser != null)
                return BadRequest(new { message = "El correo ya está registrado" });

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
                description: "Usuario registrado."
            );

            return Ok(new { message = "Usuario registrado correctamente" });
        }

        // -------------------------
        // CAMBIAR ROL (SOLO ADMIN)
        // -------------------------
        [Authorize(Roles = $"{Roles.Admin}")]
        [HttpPut("change-role")]
        public async Task<IActionResult> ChangeUserRole([FromBody] ChangeUserRoleRequest request)
        {
            if (request.UserId <= 0 || string.IsNullOrWhiteSpace(request.NewRole))
                return BadRequest(new { message = "Datos inválidos" });

            var user = await _userRepository.GetById(request.UserId);

            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            user.Role = request.NewRole;
            await _userRepository.Update(user);

            var adminId = int.Parse(User.FindFirst("id")!.Value);

            await _securityLogService.LogAsync(
                actorUserId: adminId,
                targetUserId: user.Id,
                action: "CHANGE_ROLE",
                description: $"Rol cambiado a {request.NewRole}."
            );

            return Ok(new { message = "Rol actualizado correctamente" });
        }

        // -------------------------
        // LISTAR USUARIOS
        // -------------------------
        [Authorize(Roles = $"{Roles.Admin}")]
        [HttpGet("usuarios")]
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
        [HttpDelete("usuarios/{id:int}")]
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