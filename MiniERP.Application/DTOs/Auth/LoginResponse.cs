using MiniERP.Core.Interfaces;
using MiniERP.Application.Requests;
using MiniERP.Core.Entities;
using MiniERP.Application.Interfaces;
using MiniERP.Application.DTOs.Auth;
using MiniERP.Core.Constants;

namespace MiniERP.Application.UseCases.Auth;
public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}