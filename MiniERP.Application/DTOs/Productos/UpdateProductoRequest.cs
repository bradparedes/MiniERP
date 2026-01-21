namespace MiniERP.Application.DTOs.Productos
{
    public class UpdateProductoRequest
    {
        public string Nombre { get; set; } = null!;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
    }
}
