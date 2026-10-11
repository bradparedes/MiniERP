using System.Collections.Generic;
using System.Threading.Tasks;
using MiniERP.Core.Entities;
using MiniERP.Core.Interfaces;
using MiniERP.Application.DTOs.Auth; // Asumiendo que UserDto o similar está aquí

namespace MiniERP.Application.UseCases.Auth;

public class GetUsersUseCases
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUsersUseCases(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<List<User>> Execute()
    {
        // Usar GetAllAsync
        return await _unitOfWork.Users.GetAllAsync();
    }
}