using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace CorporateWebsite.Web.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var correlationId = Guid.NewGuid().ToString("N")[..8];
        context.Response.ContentType = "application/json";
        
        var (statusCode, message, details) = exception switch
        {
            KeyNotFoundException => (HttpStatusCode.NotFound, "Resource not found", exception.Message),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized access", "You don't have permission to access this resource."),
            InvalidOperationException => (HttpStatusCode.BadRequest, "Invalid operation", exception.Message),
            ArgumentException => (HttpStatusCode.BadRequest, "Invalid argument", exception.Message),
            NotSupportedException => (HttpStatusCode.BadRequest, "Not supported", exception.Message),
            TimeoutException => (HttpStatusCode.RequestTimeout, "Request timeout", "The operation timed out."),
            _ => (HttpStatusCode.InternalServerError, "Internal server error", _env.IsDevelopment() ? exception.Message : "An unexpected error occurred.")
        };

        context.Response.StatusCode = (int)statusCode;

        _logger.LogError(exception, "Unhandled exception [CorrelationId: {CorrelationId}]: {Message}", correlationId, exception.Message);

        var response = new
        {
            error = new
            {
                code = statusCode.ToString(),
                message = message,
                details = _env.IsDevelopment() ? details : null,
                correlationId = correlationId,
                path = context.Request.Path,
                method = context.Request.Method,
                timestamp = DateTime.UtcNow
            }
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}