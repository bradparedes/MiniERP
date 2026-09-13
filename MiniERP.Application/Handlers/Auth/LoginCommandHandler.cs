using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MiniERP.Application.Commands.Auth;
using MiniERP.Core.Entities;
using MiniERP.Core.Interfaces;

namespace MiniERP.Application.Handlers.Auth
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly ISecurityLogService _securityLogService;

        public LoginCommandHandler(
            IUnitOfWork unitOfWork,
            ITokenService tokenService,
            ISecurityLogService securityLogService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _securityLogService = securityLogService ?? throw new ArgumentNullException(nameof(securityLogService));
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Corregido a GetByEmail
            var user = await _unitOfWork.Users.GetByEmail(request.Email);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid credentials.");

            if (user.PasswordHash != PasswordHasher.Hash(request.Password))
                throw new UnauthorizedAccessException("Invalid credentials.");

            var token = _tokenService.GenerateToken(user);

            // Firma de 4 parámetros limpia
            await _securityLogService.LogAsync(
                user.Id,
                user.Id,
                "LOGIN",
                $"User {user.Email} logged in successfully."
            );

            return new LoginResponse(
                Token: token,
                UserId: user.Id,
                Email: user.Email,
                Role: user.Role
            );
        }
    }
}
