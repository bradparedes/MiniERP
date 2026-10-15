using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniERP.Application.Commands.Auth;
using MiniERP.Application.DTOs.Auth;
using MiniERP.Application.Requests;
using MiniERP.Core.Constants;
using MiniERP.Application.Queries.Auth;
namespace MiniERP.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // =====================
    // LOGIN
    // =====================
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // Mapeo a Command (separación HTTP / Application)
        var command = new LoginCommand(request.Email, request.Password);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // =====================
    // REGISTER (Usuario estándar)
    // =====================
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var command = new RegisterCommand(request.Name, request.Email, request.Password);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // =====================
    // REGISTER ADMIN (Solo Admin)
    // =====================
    [Authorize(Roles = Roles.Admin)]
    [HttpPost("register-admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterRequest request)
    {
        var adminId = int.Parse(User.FindFirst("id")!.Value);
        var command = new RegisterAdminCommand(request.Name, request.Email, request.Password, adminId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // =====================
    // ELIMINAR USUARIO (Soft Delete)
    // =====================
    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("users/{userId}")]
    public async Task<IActionResult> DeleteUser(int userId, [FromQuery] int adminId)
    {
        // ✅ AdminId se extrae del Token JWT, NO de la URL ni Query
        var adminIdClaim = User.FindFirst("id")?.Value;
        if (!int.TryParse(adminIdClaim, out var adminIdFromToken) || adminIdFromToken != adminId)
            return Unauthorized(new { Error = "Invalid admin token." });

        var command = new DeleteUserCommand(adminId, userId);
        var result =await _mediator.Send(command);
        
        return Ok(result);
    }

    // =====================
    // CAMBIAR ROL
    // =====================
    [Authorize(Roles = Roles.Admin)]
    [HttpPut("change-role")]
    public async Task<IActionResult> ChangeRole([FromBody] ChangeUserRoleRequest request)
    {
        var adminIdClaim = User.FindFirst("id")?.Value;
        if (!int.TryParse(adminIdClaim, out var adminId))
            return Unauthorized();

        var command = new ChangeUserRoleCommand(request.UserId, request.NewRole, adminId);
        await _mediator.Send(command);
        return Ok(new { message = "Role updated successfully." });
    }

    // =====================
    // LISTAR USUARIOS
    // =====================
    [Authorize(Roles = Roles.Admin)]
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        // Asume que tienes GetAllUsersQuery; si no, puedes usar el handler o servicio
        var query = new GetAllUsersQuery();
        var result = await _mediator.Send(query);
        return Ok(new { message = "Users list", data = result });
    }

    // =====================
    // CAMBIAR CONTRASEÑA (Usuario autenticado)
    // =====================
    [Authorize]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var command = new ChangePasswordCommand(request.CurrentPassword, request.NewPassword, userId);
        await _mediator.Send(command);
        return Ok(new { message = "Password changed successfully." });
    }

    // =====================
    // PERFIL DEL USUARIO ACTUAL
    // =====================
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var userId = User.FindFirst("id")?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        return Ok(new
        {
            id = userId,
            email = email,
            role = role
        });
    }
}