using MiniERP.Application.DTOs.Productos;

namespace MiniERP.Application.Interfaces
{
    public interface IProductoService
    {
        Task<List<ProductoResponse>> GetAllAsync();
        Task<ProductoResponse?> GetByIdAsync(int id);
        Task<ProductoResponse> CreateAsync(CreateProductoRequest request);

        Task<ProductoResponse> UpdateAsync(int id, UpdateProductoRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
