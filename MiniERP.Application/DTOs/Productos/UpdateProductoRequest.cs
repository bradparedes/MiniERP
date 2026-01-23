namespace MiniERP.Application.DTOs.Productos
{
    public class UpdateProductoRequest
    {
        public string Nombre { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; }
    }
}
