using MiniERP.Application.DTOs.Productos;


namespace MiniERP.Application.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductResponse>> GetAllAsync();
        Task<ProductResponse?> GetByIdAsync(int id);
        Task<ProductResponse> CreateAsync(CreateProductoRequest request);

        Task<ProductResponse> UpdateAsync(int id, UpdateProductRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
