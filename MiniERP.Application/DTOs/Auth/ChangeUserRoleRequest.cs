namespace MiniERP.Application.DTOs.Auth
{
    public class ChangeUserRoleRequest
    {
        public int UserId { get; set; }
        public string NewRole { get; set; } = null!;
    }
}
