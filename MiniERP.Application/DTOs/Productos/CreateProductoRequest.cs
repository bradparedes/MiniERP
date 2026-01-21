using MiniERP.Application.DTOs.Productos;

namespace MiniERP.Application.DTOs.Productos
{
    public class CreateProductoRequest
    {
        public required string Nombre { get; set; } = null!;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
    }
}
