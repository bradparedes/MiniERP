using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MiniERP.Application.Commands.Auth;
using MiniERP.Core.Entities;
using MiniERP.Core.Interfaces;
using MiniERP.Core.Constants;

namespace MiniERP.Application.Handlers.Auth
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISecurityLogService _securityLogService;

        public RegisterCommandHandler(
            IUnitOfWork unitOfWork,
            ISecurityLogService securityLogService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _securityLogService = securityLogService ?? throw new ArgumentNullException(nameof(securityLogService));
        }

        public async Task<bool> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // Corregido a GetByEmail
            var emailExists = await _unitOfWork.Users.GetByEmail(request.Email) != null;
            if (emailExists)
                throw new InvalidOperationException("Email already registered.");

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = PasswordHasher.Hash(request.Password),
                Role = Roles.User,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.Add(user);
            await _unitOfWork.SaveChangesAsync();

            // Firma limpia de 4 parámetros
            await _securityLogService.LogAsync(
                user.Id,
                user.Id,
                "REGISTER_USER",
                $"New user registered: {user.Email}"
            );

            return true;
        }
    }
}
