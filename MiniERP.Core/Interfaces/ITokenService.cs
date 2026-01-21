using MiniERP.Core.Entities;

namespace MiniERP.Core.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
