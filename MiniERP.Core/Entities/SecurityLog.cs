using System;

namespace MiniERP.Core.Entities
{
    public class SecurityLog
    {
        public int Id { get; set; }
        public int? ActorUserId { get; set; }   // Quién realizó la acción
        public int? TargetUserId { get; set; }  // A quién afectó
        public string Action { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
