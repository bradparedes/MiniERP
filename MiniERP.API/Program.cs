using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MiniERP.Core.Interfaces;
using MiniERP.Core.Settings;
using MiniERP.Application.Interfaces;
using MiniERP.Infrastructure.Services;
using MiniERP.Infrastructure;
using MiniERP.Infrastructure.Data;
using FluentValidation;
using FluentValidation.AspNetCore;
using MiniERP.Application.Validators.Productos;
using System.Text;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using MiniERP.Core.Entities;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// =====================
// CONFIGURACIÓN
// =====================
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// =====================
// JWT SETTINGS
// =====================
var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
var jwtSettings = jwtSettingsSection.Get<JwtSettings>()
    ?? throw new Exception("No se pudo cargar JwtSettings desde appsettings.json.");

builder.Services.Configure<JwtSettings>(jwtSettingsSection);
builder.Services.AddSingleton(jwtSettings);

// =====================
// DATABASE
// =====================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// =====================
// SERVICES
// =====================

// Interfaces del Core
builder.Services.AddScoped<ITokenService, TokenService>();
// Interfaces del Producto
builder.Services.AddScoped<IProductoService, ProductoService>();
// Registra TODO Infrastructure (ProductoService, DbContext, etc.)
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<ISecurityLogService, SecurityLogService>();

builder.Services.AddScoped<ICategoriaService, CategoriaService>();


// =====================
// CONTROLLERS
// =====================
builder.Services.AddControllers()
    .AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();

builder.Services.AddValidatorsFromAssemblyContaining<MiniERP.Application.Validators.Productos.CreateProductoRequestValidator>();

// =====================
// AUTH JWT
// =====================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecretKey)
            ),
            RoleClaimType = ClaimTypes.Role
        };
    });

// =====================
// SWAGGER + JWT
// =====================
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MiniERP API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Agrega: Bearer {tu_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// =====================
// MIDDLEWARE
// =====================
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Ruta test
app.MapGet("/", () => "Hola MiniERP");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbInitializer.SeedAsync(db);
}

app.Run();
