using Microsoft.EntityFrameworkCore;
using MiniERP.Application.DTOs.Categorias;
using MiniERP.Application.Interfaces;
using MiniERP.Core.Entities;
using MiniERP.Infrastructure.Data;

namespace MiniERP.Infrastructure.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly AppDbContext _db;

        public CategoriaService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<CategoriaResponse>> GetAllAsync()
        {
            return await _db.Categorias
                .AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.Nombre)
                .Select(c => new CategoriaResponse
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Descripcion = c.Descripcion,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<CategoriaResponse?> GetByIdAsync(int id)
        {
            var categoria = await _db.Categorias
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (categoria == null)
                return null;

            return new CategoriaResponse
            {
                Id = categoria.Id,
                Nombre = categoria.Nombre,
                Descripcion = categoria.Descripcion,
                IsActive = categoria.IsActive,
                CreatedAt = categoria.CreatedAt
            };
        }

        public async Task<CategoriaResponse> CreateAsync(CreateCategoriaRequest request)
        {
            var nombreNormalizado = request.Nombre.Trim();

            var exists = await _db.Categorias
                .AnyAsync(c => c.Nombre == nombreNormalizado && c.IsActive);

            if (exists)
                throw new InvalidOperationException("Ya existe una categoría con ese nombre.");

            var categoria = new Categoria
            {
                Nombre = nombreNormalizado,
                Descripcion = request.Descripcion,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _db.Categorias.Add(categoria);
            await _db.SaveChangesAsync();

            return new CategoriaResponse
            {
                Id = categoria.Id,
                Nombre = categoria.Nombre,
                Descripcion = categoria.Descripcion,
                IsActive = categoria.IsActive,
                CreatedAt = categoria.CreatedAt
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateCategoriaRequest request)
        {
            var categoria = await _db.Categorias.FindAsync(id);

            if (categoria == null || !categoria.IsActive)
                return false;

            categoria.Nombre = request.Nombre.Trim();
            categoria.Descripcion = request.Descripcion;
            categoria.IsActive = request.IsActive;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var categoria = await _db.Categorias.FindAsync(id);
            var tieneProductos = await _db.Productos.AnyAsync(p => p.CategoriaId == id);

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
