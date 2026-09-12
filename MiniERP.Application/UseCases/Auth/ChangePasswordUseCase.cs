using System;
using System.Threading.Tasks;
using MiniERP.Application.DTOs.Auth;
using MiniERP.Application.Exceptions;
using MiniERP.Core.Entities;
using MiniERP.Core.Interfaces;

namespace MiniERP.Application.UseCases.Auth
{
    public class ChangePasswordUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISecurityLogService _securityLogService;

        public ChangePasswordUseCase(
            IUnitOfWork unitOfWork,
            ISecurityLogService securityLogService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _securityLogService = securityLogService ?? throw new ArgumentNullException(nameof(securityLogService));
        }

        public async Task Execute(ChangePasswordRequest request, int userId)
        {
            if (string.IsNullOrWhiteSpace(request.CurrentPassword) ||
                string.IsNullOrWhiteSpace(request.NewPassword))
            {
                throw new BadRequestException("Current password and new password are required.");
            }

            // 1. Obtener usuario vía UnitOfWork usando el método real
            var user = await _unitOfWork.Users.GetById(userId);

            if (user == null)
                throw new UnauthorizedException("User not found.");

            // 2. Verificar contraseña actual usando el método Hash de tu utilidad
            if (user.PasswordHash != PasswordHasher.Hash(request.CurrentPassword))
                throw new BadRequestException("Current password is incorrect.");

            // 3. Encriptar y actualizar la nueva contraseña
            user.PasswordHash = PasswordHasher.Hash(request.NewPassword);

            // 4. Persistir cambios de forma atómica en PostgreSQL
            await _unitOfWork.SaveChangesAsync();

            // 5. Registrar log de seguridad con la firma nativa correcta
            await _securityLogService.LogAsync(
                userId,
                userId,
                "CHANGE_PASSWORD",
                "User changed their password."
            );
        }
    }
}