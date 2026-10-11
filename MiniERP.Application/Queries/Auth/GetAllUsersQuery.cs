using MediatR;
using System.Collections.Generic;
using MiniERP.Application.DTOs.Auth;

namespace MiniERP.Application.Queries.Auth;

public record GetAllUsersQuery : IRequest<List<UserResponse>>;

// Si no tienes UserResponse, usa:
public record UserResponse(int Id, string Email, string Role);