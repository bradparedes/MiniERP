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

        public async Task<IEnumerable<Producto>> GetAllAsync()
        {
            return await _db.Productos
            .Select(p => new Producto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Precio = p.Precio,
                    Stock = p.Stock
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Producto> CreateAsync(CreateProductoRequest request)
        {
            var producto = new Producto
            {
                Nombre = request.Nombre,
                Precio = request.Precio,
                Stock = request.Stock
            };

            await _db.Productos.AddAsync(producto);
            await _db.SaveChangesAsync();

            return new Producto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Precio = producto.Precio,
                Stock = producto.Stock
            };
        }
        public async Task<bool> UpdateAsync(int id, UpdateProductoRequest request)
        {
            var producto = await _db.Productos.FindAsync(id);
            if (producto == null)
                return false;

            producto.Nombre = request.Nombre;
            producto.Precio = request.Precio;
            producto.Stock = request.Stock;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var producto = await _db.Productos.FindAsync(id);
            if (producto == null)
                return false;

            _db.Productos.Remove(producto);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
