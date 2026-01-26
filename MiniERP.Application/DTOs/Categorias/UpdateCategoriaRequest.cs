namespace MiniERP.Application.DTOs.Categorias
{
    public class UpdateCategoriaRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool IsActive { get; set; }
    }
}
