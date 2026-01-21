using MiniERP.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace MiniERP.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.Users.AnyAsync())
                return;

            var admin = new User
            {
                Email = "admin@minierp.com",
                PasswordHash = Hash("Admin123!"),
                Role = "Admin"
            };

            context.Users.Add(admin);
            await context.SaveChangesAsync();
        }

        private static string Hash(string password)
        {
            using var sha = SHA256.Create();
            return Convert.ToBase64String(
                sha.ComputeHash(Encoding.UTF8.GetBytes(password))
            );
        }
    }
}
