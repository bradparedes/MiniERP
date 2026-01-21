using System.ComponentModel.DataAnnotations;
namespace MiniERP.Core.Entities
{
    public class RegisterRequest
    {
        [Required]
        [MinLength(3)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;
        
        public string Role { get; set; } = "User";
    }
}
