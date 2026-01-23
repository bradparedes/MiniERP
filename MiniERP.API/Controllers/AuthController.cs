using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniERP.Core.Entities;
using MiniERP.Core.Interfaces;
using MiniERP.Infrastructure.Data;
using MiniERP.Core.Constants;
using MiniERP.Application.DTOs.Auth;

namespace MiniERP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ITokenService _tokenService;
        private readonly ISecurityLogService _securityLogService;

        public AuthController(
            AppDbContext db,
            ITokenService tokenService,
            ISecurityLogService securityLogService)
        {
            _db = db;
            _tokenService = tokenService;
            _securityLogService = securityLogService;
        }

        // -------------------------
        // LOGIN
        // -------------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { message = "Email y contraseña son obligatorios" });

            var email = request.Email.Trim().ToLower();

            var user = await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email);

            if (user == null || !PasswordHasher.Verify(request.Password, user.PasswordHash))
                return Unauthorized(new { message = "Credenciales incorrectas" });

            var token = _tokenService.GenerateToken(user);

            await _securityLogService.LogAsync(
                actorUserId: user.Id,
                targetUserId: user.Id,
                action: "LOGIN",
                description: "Inicio de sesión exitoso."
            );

            return Ok(new
            {
                message = "Login exitoso",
                token,
                user = new
                {
                    id = user.Id,
                    email = user.Email,
                    role = user.Role
                }
            });
        }

        // -------------------------
        // USUARIO LOGUEADO
        // -------------------------
        [Authorize]
        [HttpGet("usuario-logueado")]
        public IActionResult UsuarioLogueado()
        {
            var userId = User.FindFirst("id")?.Value;
            var email = User.FindFirst("email")?.Value;
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            return Ok(new
            {
                message = "Estás autenticado",
                user = new
                {
                    id = userId,
                    email,
                    role
                }
            });
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

            bool emailExists = await _db.Users
                .AnyAsync(u => u.Email.ToLower() == email);

            if (emailExists)
                return BadRequest(new { message = "El correo ya está registrado" });

            var user = new User
            {
                Email = email,
                PasswordHash = PasswordHasher.Hash(request.Password),
                Role = Roles.User
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            await _securityLogService.LogAsync(
                actorUserId: null,
                targetUserId: user.Id,
                action: "REGISTER_USER",
                description: "Usuario registrado."
            );

            return Ok(new { message = "Usuario registrado correctamente" });
        }

        // -------------------------
        // REGISTER ADMIN (SOLO ADMIN)
        // -------------------------
        [Authorize(Roles = $"{Roles.Admin}")]
        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { message = "Email y contraseña son obligatorios" });

            var email = request.Email.Trim().ToLower();

            bool emailExists = await _db.Users
                .AnyAsync(u => u.Email.ToLower() == email);

            if (emailExists)
                return BadRequest(new { message = "El correo ya está registrado" });

            var admin = new User
            {
                Email = email,
                PasswordHash = PasswordHasher.Hash(request.Password),
                Role = Roles.Admin
            };

            _db.Users.Add(admin);
            await _db.SaveChangesAsync();

            var adminId = int.Parse(User.FindFirst("id")!.Value);

            await _securityLogService.LogAsync(
                actorUserId: adminId,
                targetUserId: admin.Id,
                action: "REGISTER_ADMIN",
                description: "Administrador creó otro administrador."
            );

            return Ok(new { message = "Administrador creado correctamente" });
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

            var newRole = request.NewRole.Trim();

            if (newRole != Roles.User && newRole != Roles.Admin)
                return BadRequest(new { message = "Rol inválido. Usa 'User' o 'Admin'" });

            var user = await _db.Users.FindAsync(request.UserId);

            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            user.Role = newRole;
            await _db.SaveChangesAsync();

            var adminId = int.Parse(User.FindFirst("id")!.Value);

            await _securityLogService.LogAsync(
                actorUserId: adminId,
                targetUserId: user.Id,
                action: "CHANGE_ROLE",
                description: $"Rol cambiado a {newRole}."
            );

            return Ok(new
            {
                message = "Rol actualizado correctamente",
                user = new
                {
                    id = user.Id,
                    email = user.Email,
                    role = user.Role
                }
            });
        }

        // -------------------------
        // LISTAR USUARIOS (SOLO ADMIN)
        // -------------------------
        [Authorize(Roles = $"{Roles.Admin}")]
        [HttpGet("usuarios")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _db.Users
                .AsNoTracking()
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.Role,
                    u.CreatedAt
                })
                .ToListAsync();

            return Ok(new
            {
                message = "Lista de usuarios",
                data = users
            });
        }

        // -------------------------
        // ELIMINAR USUARIO (SOLO ADMIN)
        // -------------------------
        [Authorize(Roles = $"{Roles.Admin}")]
        [HttpDelete("usuarios/{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _db.Users.FindAsync(id);

            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            // 🔐 Blindar último admin
            if (user.Role == Roles.Admin)
            {
                var adminCount = await _db.Users.CountAsync(u => u.Role == Roles.Admin);
                if (adminCount <= 1)
                    return BadRequest(new { message = "No se puede eliminar al último administrador del sistema" });
            }

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();

            var adminId = int.Parse(User.FindFirst("id")!.Value);

            await _securityLogService.LogAsync(
                actorUserId: adminId,
                targetUserId: user.Id,
                action: "DELETE_USER",
                description: "Usuario eliminado por administrador."
            );

            return Ok(new { message = "Usuario eliminado correctamente" });
        }

        // -------------------------
        // CAMBIAR CONTRASEÑA (USUARIO LOGUEADO)
        // -------------------------
        [Authorize]
        [HttpPost("cambiar-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CurrentPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
                return BadRequest(new { message = "Contraseña actual y nueva son obligatorias" });

            var userIdClaim = User.FindFirst("id")?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized(new { message = "Token inválido" });

            int userId = int.Parse(userIdClaim);

            var user = await _db.Users.FindAsync(userId);
            if (user == null)
                return Unauthorized(new { message = "Usuario no encontrado" });

            if (!PasswordHasher.Verify(request.CurrentPassword, user.PasswordHash))
                return BadRequest(new { message = "La contraseña actual no es correcta" });

            user.PasswordHash = PasswordHasher.Hash(request.NewPassword);
            await _db.SaveChangesAsync();

            await _securityLogService.LogAsync(
                actorUserId: user.Id,
                targetUserId: user.Id,
                action: "CHANGE_PASSWORD",
                description: "El usuario cambió su contraseña."
            );

            return Ok(new { message = "Contraseña actualizada correctamente" });
        }

        // -------------------------
        // RESET PASSWORD (SOLO ADMIN)
        // -------------------------
        [Authorize(Roles = $"{Roles.Admin}")]
        [HttpPost("reset-password/{id:int}")]
        public async Task<IActionResult> ResetPassword(int id, [FromBody] ResetPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NewPassword))
                return BadRequest(new { message = "La nueva contraseña es obligatoria" });

            var user = await _db.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            user.PasswordHash = PasswordHasher.Hash(request.NewPassword);
            await _db.SaveChangesAsync();

            var adminId = int.Parse(User.FindFirst("id")!.Value);

            await _securityLogService.LogAsync(
                actorUserId: adminId,
                targetUserId: user.Id,
                action: "RESET_PASSWORD",
                description: "Administrador reseteó la contraseña de un usuario."
            );

            return Ok(new { message = "Contraseña reseteada correctamente" });
        }
    }
}
