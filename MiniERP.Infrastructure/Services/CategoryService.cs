using Microsoft.EntityFrameworkCore;
using MiniERP.Application.DTOs.Categories;
using MiniERP.Application.Requests.Categories;
using MiniERP.Application.Interfaces;
using MiniERP.Core.Entities;
using MiniERP.Infrastructure.Data;

namespace MiniERP.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _db;

        public CategoryService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<CategoryResponse>> GetAllAsync()
        {
            return await _db.Categories
                .AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .Select(c => new CategoryResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<CategoryResponse?> GetByIdAsync(int id)
        {
            var category = await _db.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (category == null)
                return null;

            return new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt
            };
        }

        public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request)
        {
            var nombreNormalizado = request.Name.Trim();

            var exists = await _db.Categories
                .AnyAsync(c => c.Name == nombreNormalizado && c.IsActive);

            if (exists)
                throw new InvalidOperationException("Ya existe una categoría con ese nombre.");

            var category = new Category
            {
                Name = nombreNormalizado,
                Description = request.Description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _db.Categories.Add(category);
            await _db.SaveChangesAsync();

            return new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateCategoryRequest request)
        {
            var category = await _db.Categories.FindAsync(id);

            if (category == null || !category.IsActive)
                return false;

            category.Name = request.Name.Trim();
            category.Description = request.Description;
            category.IsActive = request.IsActive;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _db.Categories.FindAsync(id);
            var tieneProductos = await _db.Products.AnyAsync(p => p.CategoryId == id);

            if (category == null || !category.IsActive)
                return false;
            if (tieneProductos)
                throw new InvalidOperationException("No se puede eliminar la categoría porque tiene productos asociados.");
            // 🔐 Soft delete
            category.IsActive = false;
            await _db.SaveChangesAsync();

            return true;
        }
    }
}
