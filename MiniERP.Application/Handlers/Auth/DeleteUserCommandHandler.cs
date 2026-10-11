using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MiniERP.Application.Commands.Auth;
using MiniERP.Core.Interfaces;
using MiniERP.Core.Entities;
using Microsoft.Extensions.Logging;

namespace MiniERP.Application.Handlers.Auth;

public class DeleteUserCommandHandler(
    IUnitOfWork unitOfWork,
    ISecurityLogService securityLogService,
    ILogger<DeleteUserCommandHandler> _logger) 
    : IRequestHandler<DeleteUserCommand, bool>
{
    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var userToDelete = await unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
        
        if (userToDelete == null)
        {
            _logger.LogWarning("DeleteUser failed: User {UserId} not found.", request.UserId);
            throw new KeyNotFoundException("User not found.");
        }

        await unitOfWork.Users.DeleteAsync(userToDelete, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await securityLogService.LogAsync(
            request.AdminId,
            request.UserId,
            "DELETE_USER",
            $"Admin {request.AdminId} deleted user {request.UserId}."
        );

        _logger.LogInformation("User {UserId} deleted by Admin {AdminId}.", request.UserId, request.AdminId);
        
        return true;
    }
}