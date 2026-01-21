using MiniERP.Core.Entities;
using MiniERP.Application.DTOs.Productos;
using MiniERP.Application.Interfaces;

namespace MiniERP.Application.Interfaces
{
    public interface IProductoService
    {
        Task<IEnumerable<Producto>> GetAllAsync();
        Task<Producto> CreateAsync(CreateProductoRequest request);

        Task<bool> UpdateAsync(int id, UpdateProductoRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
