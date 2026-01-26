using MiniERP.Application.DTOs.Categorias;

namespace MiniERP.Application.Interfaces
{
    public interface ICategoriaService
    {
        Task<List<CategoriaResponse>> GetAllAsync();
        Task<CategoriaResponse?> GetByIdAsync(int id);
        Task<CategoriaResponse> CreateAsync(CreateCategoriaRequest request);
        Task<bool> UpdateAsync(int id, UpdateCategoriaRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
