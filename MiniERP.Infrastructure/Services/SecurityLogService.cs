using System.Threading.Tasks;
using MiniERP.Core.Entities;
using MiniERP.Core.Interfaces;
using MiniERP.Infrastructure.Data;
using System;

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
            try
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
            catch 
            {
                //No romper la app por un log
            }
        }
    }
}
