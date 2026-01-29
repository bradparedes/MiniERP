public class ProductoResponse
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public int CategoriaId { get; set; }
    public string CategoriaNombre { get; set; } = null!;
}
