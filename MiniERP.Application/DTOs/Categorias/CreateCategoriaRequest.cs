namespace MiniERP.Application.DTOs.Categorias
{
    public class CreateCategoriaRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }
}
