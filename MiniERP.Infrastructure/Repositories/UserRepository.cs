using Microsoft.EntityFrameworkCore;
using MiniERP.Core.Entities;
using MiniERP.Core.Interfaces;
using MiniERP.Infrastructure.Data;

namespace MiniERP.Infrastructure.Repositories;
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmail(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }
}