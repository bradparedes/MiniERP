using System.Threading.Tasks;

namespace MiniERP.Core.Interfaces
{
    public interface ISecurityLogService
    {
        Task LogAsync(int? actorUserId, int? targetUserId, string action, string description);
    }
}
