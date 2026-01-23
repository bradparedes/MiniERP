namespace MiniERP.Core.Entities
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
