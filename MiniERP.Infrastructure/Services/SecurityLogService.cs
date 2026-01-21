using System.Threading.Tasks;
using MiniERP.Core.Entities;
using MiniERP.Core.Interfaces;
using MiniERP.Infrastructure.Data;

namespace MiniERP.Infrastructure.Services
{
    public class SecurityLogService : ISecurityLogService
    {
        private readonly AppDbContext _db;

        public SecurityLogService(AppDbContext db)
        {
            _db = db;
        }

        public async Task LogAsync(int? actorUserId, int? targetUserId, string action, string description)
        {
            var log = new SecurityLog
            {
                ActorUserId = actorUserId,
                TargetUserId = targetUserId,
                Action = action,
                Description = description,
                CreatedAt = DateTime.UtcNow
            };

            _db.SecurityLogs.Add(log);
            await _db.SaveChangesAsync();
        }
    }
}
