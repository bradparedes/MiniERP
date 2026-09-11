using System;
using System.Threading.Tasks;

namespace MiniERP.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        Task<int> SaveChangesAsync();
    }
}