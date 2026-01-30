namespace MiniERP.Application.DTOs.Categorias
{
    public class CreateCategoryRequest
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}
