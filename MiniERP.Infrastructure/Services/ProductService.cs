using Microsoft.EntityFrameworkCore;
using MiniERP.Application.DTOs.Products;
using MiniERP.Application.Requests.Products;
using MiniERP.Application.Interfaces;
using MiniERP.Core.Entities;
using MiniERP.Infrastructure.Data;
using MiniERP.Infrastructure.Services;

namespace MiniERP.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _db;

        public ProductService(AppDbContext db)
        {
            _db = db;
        }

        // 📦 Obtener todos los productos activos
        public async Task<List<ProductResponse>> GetAllAsync()
        {
            return await _db.Products
                .AsNoTracking()
                .Where(p => p.IsActive)
                .Include(p => p.Category)
                .Select(p => new ProductResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Stock = p.Stock,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category!.Name,
                    IsActive = p.IsActive,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();
        }

        // 🔍 Obtener producto por Id
        public async Task<ProductResponse?> GetByIdAsync(int id)
        {
            var product = await _db.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            if (product == null)
                return null;

            return new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                CategoryName = product.Category!.Name,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt
            };
        }

        // 🆕 Crear producto
        public async Task<ProductResponse> CreateAsync(CreateProductRequest request)
        {
            var category = await _db.Categories
                .FirstOrDefaultAsync(c => c.Id == request.CategoryId && c.IsActive);

            if (category is null)
                throw new InvalidOperationException("La categoría no existe o está inactiva.");

            var product = new Product
            {
                Name = request.Name.Trim(),
                Description = request.Description,
                Price = request.Price,
                Stock = request.Stock,
                CategoryId = request.CategoryId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            return new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                CategoryName = category.Name,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt
            };
        }

        // ✏️ Actualizar producto
        public async Task<ProductResponse> UpdateAsync(int id, UpdateProductRequest request)
        {
            var product = await _db.Products.FindAsync(id);

            if (product == null || !product.IsActive)
                throw new InvalidOperationException("El producto no existe o está inactivo.");

            var category = await _db.Categories
                .FirstOrDefaultAsync(c => c.Id == request.CategoryId && c.IsActive);

            if (category == null)
                throw new InvalidOperationException("La categoría no existe o está inactiva.");

            product.Name = request.Name.Trim();
            product.Description = request.Description;
            product.Price = request.Price;
            product.Stock = request.Stock;
            product.CategoryId = request.CategoryId;
            product.IsActive = request.IsActive;

            await _db.SaveChangesAsync();

            return new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                CategoryName = category.Name,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt
            };
        }

        // 🗑️ Eliminar producto (soft delete)
        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null || !product.IsActive)
                return false;

            product.IsActive = false;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
