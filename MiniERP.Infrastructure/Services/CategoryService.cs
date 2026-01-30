using Microsoft.EntityFrameworkCore;
using MiniERP.Application.DTOs.Categorias;
using MiniERP.Application.Interfaces;
using MiniERP.Core.Entities;
using MiniERP.Infrastructure.Data;
using MiniERP.Infrastructure.Migrations;

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
            return await _db.Categorias
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
            var categoria = await _db.Categorias
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (categoria == null)
                return null;

            return new CategoryResponse
            {
                Id = categoria.Id,
                Name = categoria.Name,
                Description = categoria.Description,
                IsActive = categoria.IsActive,
                CreatedAt = categoria.CreatedAt
            };
        }

        public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request)
        {
            var nombreNormalizado = request.Name.Trim();

            var exists = await _db.Categorias
                .AnyAsync(c => c.Name == nombreNormalizado && c.IsActive);

            if (exists)
                throw new InvalidOperationException("Ya existe una categoría con ese nombre.");

            var categoria = new Category
            {
                Name = nombreNormalizado,
                Description = request.Description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _db.Categorias.Add(categoria);
            await _db.SaveChangesAsync();

            return new CategoryResponse
            {
                Id = categoria.Id,
                Name = categoria.Name,
                Description = categoria.Description,
                IsActive = categoria.IsActive,
                CreatedAt = categoria.CreatedAt
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateCategoryRequest request)
        {
            var categoria = await _db.Categorias.FindAsync(id);

            if (categoria == null || !categoria.IsActive)
                return false;

            categoria.Name = request.Name.Trim();
            categoria.Description = request.Description;
            categoria.IsActive = request.IsActive;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var categoria = await _db.Categorias.FindAsync(id);
            var tieneProductos = await _db.Products.AnyAsync(p => p.CategoryId == id);

            if (categoria == null || !categoria.IsActive)
                return false;
            if (tieneProductos)
                throw new InvalidOperationException("No se puede eliminar la categoría porque tiene productos asociados.");
            // 🔐 Soft delete
            categoria.IsActive = false;
            await _db.SaveChangesAsync();

            return true;
        }
    }
}
