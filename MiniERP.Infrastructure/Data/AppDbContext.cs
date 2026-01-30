using Microsoft.EntityFrameworkCore;
using MiniERP.Core.Entities;

namespace MiniERP.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products => Set<Product>();

        public DbSet<SecurityLog> SecurityLogs => Set<SecurityLog>();

        public DbSet<Category> Categorias => Set<Category>();

        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.HasIndex(u => u.Email)
                    .IsUnique();

                entity.Property(u => u.Role)
                    .IsRequired();

                entity.Property(u => u.CreatedAt)
                    .HasDefaultValueSql("NOW()");
            });
            
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Productos)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SecurityLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Action).IsRequired();
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categorias");

                entity.HasKey(c => c.Id);

                entity.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasMany(c => c.Productos)
                    .WithOne(p => p.Category)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
