using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniERP.Core.Constants;
using Microsoft.EntityFrameworkCore;
using MiniERP.Infrastructure.Data;

namespace MiniERP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = Roles.Admin)]
    public class SecurityLogsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public SecurityLogsController(AppDbContext db)
        {
            _db = db;
        }

        // 🔍 Listar logs (con filtros opcionales)
        [HttpGet]
        public async Task<IActionResult> GetLogs(
            [FromQuery] int? actorUserId,
            [FromQuery] int? targetUserId,
            [FromQuery] string? action,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            var query = _db.SecurityLogs.AsQueryable();

            if (actorUserId.HasValue)
                query = query.Where(l => l.ActorUserId == actorUserId);

            if (targetUserId.HasValue)
                query = query.Where(l => l.TargetUserId == targetUserId);

            if (!string.IsNullOrWhiteSpace(action))
                query = query.Where(l => l.Action == action);

            if (from.HasValue)
                query = query.Where(l => l.CreatedAt >= from.Value);

            if (to.HasValue)
                query = query.Where(l => l.CreatedAt <= to.Value);

            var logs = await query
                .OrderByDescending(l => l.CreatedAt)
                .Take(100) // 🔒 límite de seguridad
                .Select(l => new
                {
                    l.Id,
                    l.ActorUserId,
                    l.TargetUserId,
                    l.Action,
                    l.Description,
                    l.CreatedAt
                })
                .ToListAsync();

            return Ok(new
            {
                message = "Logs de seguridad",
                data = logs
            });
        }
    }
}
