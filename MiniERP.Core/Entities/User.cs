using MiniERP.Core.Constants;

namespace MiniERP.Core.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; } = null!;
        
        public string PasswordHash { get; set; } = null!;

        public string Role { get; set; } = Roles.User + "," + Roles.Admin; // Admin o User
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
