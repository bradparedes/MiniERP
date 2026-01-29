using Microsoft.EntityFrameworkCore;
using MiniERP.Application.DTOs.Productos;
using MiniERP.Application.Interfaces;
using MiniERP.Core.Entities;
using MiniERP.Infrastructure.Data;

namespace MiniERP.Infrastructure.Services
{
    public class ProductoService : IProductoService
    {
        private readonly AppDbContext _db;

        public ProductoService(AppDbContext db)
        {
            _db = db;
        }

        // 📦 Obtener todos los productos activos
        public async Task<List<ProductoResponse>> GetAllAsync()
        {
            return await _db.Productos
                .AsNoTracking()
                .Where(p => p.IsActive)
                .Include(p => p.Categoria)
                .Select(p => new ProductoResponse
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Description = p.Description,
                    Precio = p.Precio,
                    Stock = p.Stock,
                    CategoriaId = p.CategoriaId,
                    CategoriaNombre = p.Categoria!.Nombre,
                    IsActive = p.IsActive,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();
        }

        // 🔍 Obtener producto por Id
        public async Task<ProductoResponse?> GetByIdAsync(int id)
        {
            var producto = await _db.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            if (producto == null)
                return null;

            return new ProductoResponse
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Description = producto.Description,
                Precio = producto.Precio,
                Stock = producto.Stock,
                CategoriaId = producto.CategoriaId,
                CategoriaNombre = producto.Categoria!.Nombre,
                IsActive = producto.IsActive,
                CreatedAt = producto.CreatedAt
            };
        }

        // 🆕 Crear producto
        public async Task<ProductoResponse> CreateAsync(CreateProductoRequest request)
        {
            var categoria = await _db.Categorias
                .FirstOrDefaultAsync(c => c.Id == request.CategoriaId && c.IsActive);

            if (categoria is null)
                throw new InvalidOperationException("La categoría no existe o está inactiva.");

            var producto = new Producto
            {
                Nombre = request.Nombre.Trim(),
                Description = request.Description,
                Precio = request.Precio,
                Stock = request.Stock,
                CategoriaId = request.CategoriaId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _db.Productos.Add(producto);
            await _db.SaveChangesAsync();

            return new ProductoResponse
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Description = producto.Description,
                Precio = producto.Precio,
                Stock = producto.Stock,
                CategoriaId = producto.CategoriaId,
                CategoriaNombre = categoria.Nombre,
                IsActive = producto.IsActive,
                CreatedAt = producto.CreatedAt
            };
        }

        // ✏️ Actualizar producto
        public async Task<ProductoResponse> UpdateAsync(int id, UpdateProductoRequest request)
        {
            var producto = await _db.Productos.FindAsync(id);

            if (producto == null || !producto.IsActive)
                throw new InvalidOperationException("El producto no existe o está inactivo.");

            var categoria = await _db.Categorias
                .FirstOrDefaultAsync(c => c.Id == request.CategoriaId && c.IsActive);

            if (categoria == null)
                throw new InvalidOperationException("La categoría no existe o está inactiva.");

            producto.Nombre = request.Nombre.Trim();
            producto.Description = request.Description;
            producto.Precio = request.Precio;
            producto.Stock = request.Stock;
            producto.CategoriaId = request.CategoriaId;
            producto.IsActive = request.IsActive;

            await _db.SaveChangesAsync();

            return new ProductoResponse
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Description = producto.Description,
                Precio = producto.Precio,
                Stock = producto.Stock,
                CategoriaId = producto.CategoriaId,
                CategoriaNombre = categoria.Nombre,
                IsActive = producto.IsActive,
                CreatedAt = producto.CreatedAt
            };
        }

        // 🗑️ Eliminar producto (soft delete)
        public async Task<bool> DeleteAsync(int id)
        {
            var producto = await _db.Productos.FindAsync(id);
            if (producto == null || !producto.IsActive)
                return false;

            producto.IsActive = false;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
