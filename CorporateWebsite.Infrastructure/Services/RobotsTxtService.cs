using CorporateWebsite.Core.DTOs;
using CorporateWebsite.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CorporateWebsite.Infrastructure.Services;

public interface IRobotsTxtService
{
    Task<string> GenerateRobotsTxtAsync();
    Task<RobotsTxtDto> GetRobotsConfigAsync();
}

public class RobotsTxtService : IRobotsTxtService
{
    private readonly ISettingService _settingService;
    private readonly ISitemapService _sitemapService;

    public RobotsTxtService(ISettingService settingService, ISitemapService sitemapService)
    {
        _settingService = settingService;
        _sitemapService = sitemapService;
    }

    public async Task<string> GenerateRobotsTxtAsync()
    {
        var config = await GetRobotsConfigAsync();
        var baseUrl = await _settingService.GetValueAsync("CanonicalDomain", "https://localhost");
        
        var sb = new StringBuilder();
        
        sb.AppendLine($"User-agent: {config.UserAgent}");
        
        foreach (var disallow in config.Disallow)
        {
            sb.AppendLine($"Disallow: {disallow}");
        }
        
        foreach (var allow in config.Allow)
        {
            sb.AppendLine($"Allow: {allow}");
        }
        
        if (!string.IsNullOrEmpty(config.Sitemap))
        {
            sb.AppendLine($"Sitemap: {baseUrl.TrimEnd('/')}{config.Sitemap}");
        }
        else
        {
            sb.AppendLine($"Sitemap: {baseUrl.TrimEnd('/')}/sitemap.xml");
        }
        
        foreach (var host in config.Host)
        {
            sb.AppendLine($"Host: {host}");
        }
        
        if (config.CrawlDelay.HasValue)
        {
            sb.AppendLine($"Crawl-delay: {config.CrawlDelay.Value}");
        }

        return sb.ToString();
    }

    public async Task<RobotsTxtDto> GetRobotsConfigAsync()
    {
        var robotsTxt = await _settingService.GetValueAsync("RobotsTxt", "");
        
        var config = new RobotsTxtDto
        {
            UserAgent = "*",
            Disallow = new List<string>(),
            Allow = new List<string>(),
            Sitemap = "/sitemap.xml"
        };

        if (!string.IsNullOrEmpty(robotsTxt))
        {
            var lines = robotsTxt.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            
            foreach (var line in lines)
            {
                if (line.StartsWith("User-agent:", StringComparison.OrdinalIgnoreCase))
                {
                    config.UserAgent = line.Substring("User-agent:".Length).Trim();
                }
                else if (line.StartsWith("Disallow:", StringComparison.OrdinalIgnoreCase))
                {
                    config.Disallow.Add(line.Substring("Disallow:".Length).Trim());
                }
                else if (line.StartsWith("Allow:", StringComparison.OrdinalIgnoreCase))
                {
                    config.Allow.Add(line.Substring("Allow:".Length).Trim());
                }
                else if (line.StartsWith("Sitemap:", StringComparison.OrdinalIgnoreCase))
                {
                    config.Sitemap = line.Substring("Sitemap:".Length).Trim();
                }
                else if (line.StartsWith("Host:", StringComparison.OrdinalIgnoreCase))
                {
                    config.Host.Add(line.Substring("Host:".Length).Trim());
                }
                else if (line.StartsWith("Crawl-delay:", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(line.Substring("Crawl-delay:".Length).Trim(), out var delay))
                    {
                        config.CrawlDelay = delay;
                    }
                }
            }
        }

        // Default disallows for admin and private areas
        if (!config.Disallow.Any())
        {
            config.Disallow.AddRange(new[] { "/admin/", "/api/", "/account/", "/login", "/register" });
        }

        return config;
    }
}