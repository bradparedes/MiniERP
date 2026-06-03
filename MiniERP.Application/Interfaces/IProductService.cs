using MiniERP.Application.DTOs.Products;
using MiniERP.Application.Requests.Products;


namespace MiniERP.Application.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductResponse>> GetAllAsync();
        Task<ProductResponse?> GetByIdAsync(int id);
        Task<ProductResponse> CreateAsync(CreateProductRequest request);

        Task<ProductResponse> UpdateAsync(int id, UpdateProductRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
