using MiniERP.Application.Requests.Categories;
using MiniERP.Application.DTOs.Categories;

namespace MiniERP.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryResponse>> GetAllAsync();
        Task<CategoryResponse?> GetByIdAsync(int id);
        Task<CategoryResponse> CreateAsync(CreateCategoryRequest request);
        Task<bool> UpdateAsync(int id, UpdateCategoryRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
