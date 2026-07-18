using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Text.RegularExpressions;

namespace CorporateWebsite.Web.Middleware;

public class SeoMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SeoMiddleware> _logger;
    private static readonly Regex _botRegex = new Regex(
        @"(bot|crawler|spider|scraper|archiver|facebookexternalhit|twitterbot|linkedinbot|whatsapp|telegrambot|slackbot|discordbot|redditbot|applebot|bingbot|googlebot|yandex|baiduspider|duckduckbot|slurp|ia_archiver|mediapartners|adsbot|adsbot-google|msnbot|askbot|exabot|gigabot|lycos|scooter|infoseek|webcrawler|webcrawler/1.0|webcrawler/2.0|webcrawler/3.0|webcrawler/4.0|webcrawler/5.0)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public SeoMiddleware(RequestDelegate next, ILogger<SeoMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower() ?? "";
        
        // Check for bot/crawler
        var userAgent = context.Request.Headers["User-Agent"].ToString();
        var isBot = _botRegex.IsMatch(userAgent);
        
        if (isBot)
        {
            context.Items["IsBot"] = true;
            _logger.LogDebug("Bot detected: {UserAgent} accessing {Path}", userAgent, path);
        }

        // Add canonical URL header
        var canonicalUrl = GetCanonicalUrl(context);
        if (!string.IsNullOrEmpty(canonicalUrl))
        {
            context.Response.Headers["Link"] = $"<{canonicalUrl}>; rel=\"canonical\"";
        }

        // Add hreflang headers for localized pages
        await AddHreflangHeaders(context);

        // Handle trailing slashes
        if (path != "/" && path.EndsWith("/") && !path.Contains("."))
        {
            var newPath = path.TrimEnd('/');
            var queryString = context.Request.QueryString.Value;
            context.Response.Redirect($"{newPath}{queryString}", permanent: true);
            return;
        }

        // Track 404 for broken links
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await _next(context);

        context.Response.Body = originalBodyStream;
        await responseBody.CopyToAsync(originalBodyStream);

        // Log 404 for broken link tracking
        if (context.Response.StatusCode == 404 && !isBot)
        {
            await LogBrokenLink(context, path, userAgent);
        }
    }

    private string GetCanonicalUrl(HttpContext context)
    {
        var request = context.Request;
        var host = request.Host.Value;
        var scheme = request.Scheme;
        var path = request.Path.Value?.TrimEnd('/') ?? "/";
        
        // Remove culture prefix for canonical
        var cultureMatch = Regex.Match(path, @"^/(fa|en|ar)(/.*)?$");
        if (cultureMatch.Success)
        {
            path = cultureMatch.Groups[2].Value;
            if (string.IsNullOrEmpty(path)) path = "/";
        }
        
        return $"{scheme}://{host}{path}";
    }

    private async Task AddHreflangHeaders(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower() ?? "";
        var cultureMatch = Regex.Match(path, @"^/(fa|en|ar)(/.*)?$");
        
        if (cultureMatch.Success)
        {
            var currentCulture = cultureMatch.Groups[1].Value;
            var restPath = cultureMatch.Groups[2].Value;
            if (string.IsNullOrEmpty(restPath)) restPath = "/";
            
            var host = context.Request.Host.Value;
            var scheme = context.Request.Scheme;
            var baseUrl = $"{scheme}://{host}";
            
            var hreflangs = new List<string>
            {
                $"<{baseUrl}/fa{restPath}>; rel=\"alternate\"; hreflang=\"fa\"",
                $"<{baseUrl}/en{restPath}>; rel=\"alternate\"; hreflang=\"en\"",
                $"<{baseUrl}/ar{restPath}>; rel=\"alternate\"; hreflang=\"ar\"",
                $"<{baseUrl}/{currentCulture}{restPath}>; rel=\"alternate\"; hreflang=\"x-default\""
            };
            
            context.Response.Headers["Link"] = string.Join(", ", hreflangs);
        }
    }

    private async Task LogBrokenLink(HttpContext context, string path, string userAgent)
    {
        try
        {
            var referrer = context.Request.Headers["Referer"].ToString();
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            
            _logger.LogWarning("404 - Broken link: {Path} from {Referrer} by {IP} ({UserAgent})", 
                path, referrer, ipAddress, userAgent);
            
            // In production, this would be saved to database via a service
            // await _brokenLinkService.LogAsync(path, referrer, ipAddress, userAgent);
        }
        catch
        {
            // Ignore logging errors
        }
    }
}

public static class SeoMiddlewareExtensions
{
    public static IApplicationBuilder UseSeo(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SeoMiddleware>();
    }
}