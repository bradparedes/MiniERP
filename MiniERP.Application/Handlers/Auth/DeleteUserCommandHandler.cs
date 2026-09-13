using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MiniERP.Application.Commands.Auth;
using MiniERP.Core.Entities;
using MiniERP.Core.Interfaces;

namespace MiniERP.Application.Handlers.Auth
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISecurityLogService _securityLogService;

        public DeleteUserCommandHandler(
            IUnitOfWork unitOfWork,
            ISecurityLogService securityLogService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _securityLogService = securityLogService ?? throw new ArgumentNullException(nameof(securityLogService));
        }

        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            // 1. Búsqueda del usuario afectado por ID usando tu método real
            var userToDelete = await _unitOfWork.Users.GetById(request.UserId);
            if (userToDelete == null)
                throw new KeyNotFoundException("User not found.");

            // 2. Marcar el usuario para eliminación en memoria (SIN AWAIT porque es void)
            _unitOfWork.Users.Delete(userToDelete);

            // 3. Persistir los cambios físicamente en PostgreSQL de forma asíncrona
            await _unitOfWork.SaveChangesAsync();

            // 4. Registrar auditoría asíncrona con la firma limpia de 4 parámetros
            await _securityLogService.LogAsync(
                request.AdminId,
                request.UserId,
                "DELETE_USER",
                $"Admin {request.AdminId} soft-deleted user {request.UserId}."
            );

            return true;
        }
    }
}
