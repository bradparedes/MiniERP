using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MiniERP.Core.Settings;
using MiniERP.Application.Interfaces;
using MiniERP.Infrastructure.Data;
using MiniERP.Infrastructure.Services;
using MiniERP.Core.Interfaces;
using MiniERP.Infrastructure.Repositories;

namespace MiniERP.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Configurar DbContext
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // Obtener sección JwtSettings
            var jwtSection = configuration.GetSection("JwtSettings");

            // Configurar JwtSettings de forma correcta
            services.Configure<JwtSettings>(jwtSection);

            return services;
        }
    }
}
