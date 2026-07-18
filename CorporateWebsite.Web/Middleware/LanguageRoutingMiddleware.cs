using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace CorporateWebsite.Web.Middleware;

public class LanguageRoutingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly RequestLocalizationOptions _localizationOptions;
    private readonly string[] _supportedCultures = { "fa", "en", "ar" };

    public LanguageRoutingMiddleware(RequestDelegate next, IOptions<RequestLocalizationOptions> localizationOptions)
    {
        _next = next;
        _localizationOptions = localizationOptions.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower() ?? "";
        
        // Skip for static files, api, health checks, etc.
        if (ShouldSkipLocalization(path))
        {
            await _next(context);
            return;
        }

        // Extract culture from path
        var culture = ExtractCultureFromPath(path);
        
        if (!string.IsNullOrEmpty(culture) && _supportedCultures.Contains(culture))
        {
            // Set the culture
            var cultureInfo = new CultureInfo(culture == "fa" ? "fa-IR" : culture == "en" ? "en-US" : "ar-SA");
            
            context.Features.Set<IRequestCultureFeature>(new RequestCultureFeature(
                new RequestCulture(cultureInfo, cultureInfo)));
            
            // Store culture in session for persistence
            context.Session.SetString("Culture", culture);
        }
        else if (path == "/" || path == "")
        {
            // Redirect root to default culture
            var defaultCulture = context.Session.GetString("Culture") ?? "fa";
            context.Response.Redirect($"/{defaultCulture}/");
            return;
        }

        await _next(context);
    }

    private bool ShouldSkipLocalization(string path)
    {
        var skipPrefixes = new[] { "/uploads", "/images", "/css", "/js", "/lib", "/favicon", "/health", "/api", "/error", "/robots.txt", "/sitemap.xml", "/.well-known" };
        return skipPrefixes.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase));
    }

    private string? ExtractCultureFromPath(string path)
    {
        if (string.IsNullOrEmpty(path) || path == "/")
            return null;

        var segments = path.Trim('/').Split('/');
        if (segments.Length > 0 && _supportedCultures.Contains(segments[0]))
        {
            return segments[0];
        }

        return null;
    }
}

public static class LanguageRoutingMiddlewareExtensions
{
    public static IApplicationBuilder UseLanguageRouting(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LanguageRoutingMiddleware>();
    }
}