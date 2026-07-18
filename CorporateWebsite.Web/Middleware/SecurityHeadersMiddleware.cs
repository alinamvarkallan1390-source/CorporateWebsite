using Microsoft.AspNetCore.Http;

namespace CorporateWebsite.Web.Middleware;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Security Headers
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        context.Response.Headers["X-Frame-Options"] = "DENY";
        context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
        context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
        context.Response.Headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";
        
        // CSP Header
        var csp = "default-src 'self'; " +
                  "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://www.google.com https://www.gstatic.com https://cdnjs.cloudflare.com; " +
                  "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com https://cdnjs.cloudflare.com; " +
                  "font-src 'self' https://fonts.gstatic.com https://cdnjs.cloudflare.com data:; " +
                  "img-src 'self' data: https:; " +
                  "connect-src 'self' https://www.google.com https://www.gstatic.com; " +
                  "frame-src 'self' https://www.google.com; " +
                  "form-action 'self'; " +
                  "base-uri 'self'; " +
                  "object-src 'none';";
        
        context.Response.Headers["Content-Security-Policy"] = csp;

        await _next(context);
    }
}

public static class SecurityHeadersMiddlewareExtensions
{
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SecurityHeadersMiddleware>();
    }
}