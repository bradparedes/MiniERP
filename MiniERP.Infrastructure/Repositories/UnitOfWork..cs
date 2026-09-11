using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MiniERP.Core.Interfaces;
using MiniERP.Infrastructure.Data;

namespace MiniERP.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IUserRepository? _userRepository;

        public UnitOfWork(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IUserRepository Users => _userRepository ??= new UserRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}