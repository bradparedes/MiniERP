namespace MiniERP.Application.DTOs.Categorias
{
    public class UpdateCategoriaRequest
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public bool IsActive { get; set; }
    }
}
