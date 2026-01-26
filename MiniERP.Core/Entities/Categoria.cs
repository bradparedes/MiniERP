namespace MiniERP.Core.Entities
{
    public class Categoria
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
