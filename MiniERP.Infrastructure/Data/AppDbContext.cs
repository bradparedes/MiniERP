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

        public DbSet<Category> Categories => Set<Category>();

        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(255);
                
                entity.Property(u => u.PasswordHash)
                    .IsRequired();

                entity.Property(u => u.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(u => u.Email)
                    .IsUnique();

                entity.Property(u => u.Role)
                    .IsRequired();

                entity.Property(u => u.CreatedAt)
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("NOW()");
                
                entity.Property(u => u.UpdatedAt)
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("NOW()");
            });
            
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
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
                entity.ToTable("Categories");

                entity.HasKey(c => c.Id);

                entity.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasMany(c => c.Products)
                    .WithOne(p => p.Category)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
