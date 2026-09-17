using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MiniERP.Application.Exceptions;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger; // 1. Sistema de Logs inyectado

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context) // Mantiene la firma original Invoke
    {
        try
        {
            await _next(context);
        }
        catch (UnauthorizedException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsJsonAsync(new { message = ex.Message });
            _logger.LogWarning("Unauthorized access: {Message}", ex.Message);
        }
        catch (BadRequestException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsJsonAsync(new { message = ex.Message });
            _logger.LogWarning("Bad request constraint: {Message}", ex.Message);
        }
        catch (NotFoundException ex) // Captura tus excepciones 404 NotFound
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsJsonAsync(new { message = ex.Message });
            _logger.LogWarning("Resource not found: {Message}", ex.Message);
        }
        catch (Exception ex) // Captura errores críticos del sistema (ej: Postgres caído)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsJsonAsync(new { message = "Internal server error" });
            
            // Registra el error completo en la consola de Ubuntu con todo su Stack Trace
            _logger.LogError(ex, "A critical unhandled exception occurred in the API pipeline.");
        }
    }
}
