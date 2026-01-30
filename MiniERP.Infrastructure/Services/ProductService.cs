using Microsoft.EntityFrameworkCore;
using MiniERP.Application.DTOs.Productos;
using MiniERP.Application.Interfaces;
using MiniERP.Core.Entities;
using MiniERP.Infrastructure.Data;

namespace MiniERP.Infrastructure.Services
{
    public class ProductoService : IProductService
    {
        private readonly AppDbContext _db;

        public ProductoService(AppDbContext db)
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
            var producto = await _db.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            if (producto == null)
                return null;

            return new ProductResponse
            {
                Id = producto.Id,
                Name = producto.Name,
                Description = producto.Description,
                Price = producto.Price,
                Stock = producto.Stock,
                CategoryId = producto.CategoryId,
                CategoryName = producto.Category!.Name,
                IsActive = producto.IsActive,
                CreatedAt = producto.CreatedAt
            };
        }

        // 🆕 Crear producto
        public async Task<ProductResponse> CreateAsync(CreateProductoRequest request)
        {
            var category = await _db.Categorias
                .FirstOrDefaultAsync(c => c.Id == request.CategoryId && c.IsActive);

            if (category is null)
                throw new InvalidOperationException("La categoría no existe o está inactiva.");

            var producto = new Product
            {
                Name = request.Name.Trim(),
                Description = request.Description,
                Price = request.Price,
                Stock = request.Stock,
                CategoryId = request.CategoryId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _db.Products.Add(producto);
            await _db.SaveChangesAsync();

            return new ProductResponse
            {
                Id = producto.Id,
                Name = producto.Name,
                Description = producto.Description,
                Price = producto.Price,
                Stock = producto.Stock,
                CategoryId = producto.CategoryId,
                CategoryName = category.Name,
                IsActive = producto.IsActive,
                CreatedAt = producto.CreatedAt
            };
        }

        // ✏️ Actualizar producto
        public async Task<ProductResponse> UpdateAsync(int id, UpdateProductRequest request)
        {
            var producto = await _db.Products.FindAsync(id);

            if (producto == null || !producto.IsActive)
                throw new InvalidOperationException("El producto no existe o está inactivo.");

            var category = await _db.Categorias
                .FirstOrDefaultAsync(c => c.Id == request.CategoryId && c.IsActive);

            if (category == null)
                throw new InvalidOperationException("La categoría no existe o está inactiva.");

            producto.Name = request.Name.Trim();
            producto.Description = request.Description;
            producto.Price = request.Price;
            producto.Stock = request.Stock;
            producto.CategoryId = request.CategoryId;
            producto.IsActive = request.IsActive;

            await _db.SaveChangesAsync();

            return new ProductResponse
            {
                Id = producto.Id,
                Name = producto.Name,
                Description = producto.Description,
                Price = producto.Price,
                Stock = producto.Stock,
                CategoryId = producto.CategoryId,
                CategoryName = category.Name,
                IsActive = producto.IsActive,
                CreatedAt = producto.CreatedAt
            };
        }

        // 🗑️ Eliminar producto (soft delete)
        public async Task<bool> DeleteAsync(int id)
        {
            var producto = await _db.Products.FindAsync(id);
            if (producto == null || !producto.IsActive)
                return false;

            producto.IsActive = false;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
