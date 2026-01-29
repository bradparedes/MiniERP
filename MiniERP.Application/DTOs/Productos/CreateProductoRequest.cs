namespace MiniERP.Application.DTOs.Productos
{
    public class CreateProductoRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public int CategoriaId { get; set; }
    }
}
