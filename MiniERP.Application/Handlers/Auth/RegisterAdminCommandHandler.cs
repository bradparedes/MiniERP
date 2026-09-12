using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MiniERP.Application.Commands.Auth;
using MiniERP.Core.Constants;
using MiniERP.Core.Entities;
using MiniERP.Core.Interfaces;

namespace MiniERP.Application.Handlers.Auth
{
    public class RegisterAdminCommandHandler : IRequestHandler<RegisterAdminCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISecurityLogService _securityLogService;

        public RegisterAdminCommandHandler(
            IUnitOfWork unitOfWork,
            ISecurityLogService securityLogService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _securityLogService = securityLogService ?? throw new ArgumentNullException(nameof(securityLogService));
        }

        public async Task<bool> Handle(RegisterAdminCommand request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = PasswordHasher.Hash(request.Password),
                Role = Roles.Admin,
                CreatedAt = DateTime.UtcNow
            };

            // 1. Agregar a través del repositorio del UnitOfWork
            await _unitOfWork.Users.Add(user);

            // 2. Persistir transacción de forma atómica
            await _unitOfWork.SaveChangesAsync();

            // 3. Registrar auditoría asíncrona
            await _securityLogService.LogAsync(
                actorUserId: request.AdminId,
                targetUserId: user.Id,
                action: "REGISTER_ADMIN",
                description: $"Admin {request.AdminId} registered new admin: {user.Email}"
            );

            return true;
        }
    }
}
