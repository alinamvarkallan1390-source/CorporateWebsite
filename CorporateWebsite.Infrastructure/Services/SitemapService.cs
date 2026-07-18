using CorporateWebsite.Core.DTOs;
using CorporateWebsite.Core.Entities;
using CorporateWebsite.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CorporateWebsite.Infrastructure.Services;

public interface ISitemapService
{
    Task<string> GenerateSitemapAsync();
    Task<List<SitemapUrlDto>> GetSitemapUrlsAsync();
}

public class SitemapService : ISitemapService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILanguageService _languageService;
    private readonly ISettingService _settingService;

    public SitemapService(IUnitOfWork unitOfWork, ILanguageService languageService, ISettingService settingService)
    {
        _unitOfWork = unitOfWork;
        _languageService = languageService;
        _settingService = settingService;
    }

    public async Task<string> GenerateSitemapAsync()
    {
        var urls = await GetSitemapUrlsAsync();
        var baseUrl = await _settingService.GetValueAsync("CanonicalDomain", "https://localhost");
        
        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\"");
        sb.AppendLine("        xmlns:xhtml=\"http://www.w3.org/1999/xhtml\">");

        foreach (var url in urls)
        {
            sb.AppendLine("  <url>");
            sb.AppendLine($"    <loc>{baseUrl.TrimEnd('/')}{url.Url}</loc>");
            
            if (url.LastModified.HasValue)
            {
                sb.AppendLine($"    <lastmod>{url.LastModified.Value:yyyy-MM-dd}</lastmod>");
            }
            
            if (!string.IsNullOrEmpty(url.ChangeFrequency))
            {
                sb.AppendLine($"    <changefreq>{url.ChangeFrequency}</changefreq>");
            }
            
            if (url.Priority.HasValue)
            {
                sb.AppendLine($"    <priority>{url.Priority.Value:F1}</priority>");
            }

            // Add hreflang alternates
            foreach (var alternate in url.Alternates)
            {
                sb.AppendLine($"    <xhtml:link rel=\"alternate\" hreflang=\"{alternate.Key}\" href=\"{baseUrl.TrimEnd('/')}{alternate.Value}\" />");
            }

            // Add self reference
            var defaultLang = await _languageService.GetDefaultAsync();
            if (defaultLang != null)
            {
                sb.AppendLine($"    <xhtml:link rel=\"alternate\" hreflang=\"{defaultLang.Code}\" href=\"{baseUrl.TrimEnd('/')}{url.Url}\" />");
            }

            sb.AppendLine("  </url>");
        }

        sb.AppendLine("</urlset>");
        return sb.ToString();
    }

    public async Task<List<SitemapUrlDto>> GetSitemapUrlsAsync()
    {
        var urls = new List<SitemapUrlDto>();
        var languages = await _languageService.GetAllActiveAsync();
        
        // Home page
        foreach (var lang in languages)
        {
            urls.Add(new SitemapUrlDto
            {
                Url = $"/{lang.Code}/",
                LastModified = DateTime.UtcNow,
                ChangeFrequency = "daily",
                Priority = 1.0,
                Alternates = languages.ToDictionary(l => l.Code, l => $"/{l.Code}/")
            });
        }

        // Pages
        var pages = await _unitOfWork.Pages.GetAllAsync(p => p.IsPublished, q => q.OrderBy(p => p.DisplayOrder), p => p.Translations);
        foreach (var page in pages)
        {
            foreach (var translation in page.Translations)
            {
                var alternates = page.Translations
                    .Where(t => t.LanguageId != translation.LanguageId)
                    .ToDictionary(t => languages.First(l => l.Id == t.LanguageId).Code, t => $"/{languages.First(l => l.Id == t.LanguageId).Code}/{t.Slug}");
                
                urls.Add(new SitemapUrlDto
                {
                    Url = $"/{languages.First(l => l.Id == translation.LanguageId).Code}/{translation.Slug}",
                    LastModified = page.UpdatedAt ?? page.CreatedAt,
                    ChangeFrequency = "weekly",
                    Priority = 0.8,
                    Alternates = alternates
                });
            }
        }

        // Services
        var services = await _unitOfWork.Services.GetAllAsync(s => s.IsPublished, q => q.OrderBy(s => s.DisplayOrder), s => s.Translations);
        foreach (var service in services)
        {
            foreach (var translation in service.Translations)
            {
                var alternates = service.Translations
                    .Where(t => t.LanguageId != translation.LanguageId)
                    .ToDictionary(t => languages.First(l => l.Id == t.LanguageId).Code, t => $"/{languages.First(l => l.Id == t.LanguageId).Code}/services/{t.Slug}");
                
                urls.Add(new SitemapUrlDto
                {
                    Url = $"/{languages.First(l => l.Id == translation.LanguageId).Code}/services/{translation.Slug}",
                    LastModified = service.UpdatedAt ?? service.CreatedAt,
                    ChangeFrequency = "weekly",
                    Priority = 0.8,
                    Alternates = alternates
                });
            }
        }

        // Projects
        var projects = await _unitOfWork.Projects.GetAllAsync(p => p.IsPublished, q => q.OrderBy(p => p.DisplayOrder), p => p.Translations);
        foreach (var project in projects)
        {
            foreach (var translation in project.Translations)
            {
                var alternates = project.Translations
                    .Where(t => t.LanguageId != translation.LanguageId)
                    .ToDictionary(t => languages.First(l => l.Id == t.LanguageId).Code, t => $"/{languages.First(l => l.Id == t.LanguageId).Code}/projects/{t.Slug}");
                
                urls.Add(new SitemapUrlDto
                {
                    Url = $"/{languages.First(l => l.Id == translation.LanguageId).Code}/projects/{translation.Slug}",
                    LastModified = project.UpdatedAt ?? project.CreatedAt,
                    ChangeFrequency = "weekly",
                    Priority = 0.8,
                    Alternates = alternates
                });
            }
        }

        // News
        var news = await _unitOfWork.News.GetAllAsync(n => n.IsPublished, q => q.OrderByDescending(n => n.PublishedAt), n => n.Translations);
        foreach (var newsItem in news)
        {
            foreach (var translation in newsItem.Translations)
            {
                var alternates = newsItem.Translations
                    .Where(t => t.LanguageId != translation.LanguageId)
                    .ToDictionary(t => languages.First(l => l.Id == t.LanguageId).Code, t => $"/{languages.First(l => l.Id == t.LanguageId).Code}/news/{t.Slug}");
                
                urls.Add(new SitemapUrlDto
                {
                    Url = $"/{languages.First(l => l.Id == translation.LanguageId).Code}/news/{translation.Slug}",
                    LastModified = newsItem.UpdatedAt ?? newsItem.CreatedAt,
                    ChangeFrequency = "monthly",
                    Priority = 0.7,
                    Alternates = alternates
                });
            }
        }

        // Service Categories
        var serviceCategories = await _unitOfWork.ServiceCategories.GetAllAsync(c => c.IsActive, q => q.OrderBy(c => c.DisplayOrder), c => c.Translations);
        foreach (var category in serviceCategories)
        {
            foreach (var translation in category.Translations)
            {
                var alternates = category.Translations
                    .Where(t => t.LanguageId != translation.LanguageId)
                    .ToDictionary(t => languages.First(l => l.Id == t.LanguageId).Code, t => $"/{languages.First(l => l.Id == t.LanguageId).Code}/services/category/{t.Slug}");
                
                urls.Add(new SitemapUrlDto
                {
                    Url = $"/{languages.First(l => l.Id == translation.LanguageId).Code}/services/category/{translation.Slug}",
                    LastModified = category.UpdatedAt ?? category.CreatedAt,
                    ChangeFrequency = "weekly",
                    Priority = 0.6,
                    Alternates = alternates
                });
            }
        }

        // Project Categories
        var projectCategories = await _unitOfWork.ProjectCategories.GetAllAsync(c => c.IsActive, q => q.OrderBy(c => c.DisplayOrder), c => c.Translations);
        foreach (var category in projectCategories)
        {
            foreach (var translation in category.Translations)
            {
                var alternates = category.Translations
                    .Where(t => t.LanguageId != translation.LanguageId)
                    .ToDictionary(t => languages.First(l => l.Id == t.LanguageId).Code, t => $"/{languages.First(l => l.Id == t.LanguageId).Code}/projects/category/{t.Slug}");
                
                urls.Add(new SitemapUrlDto
                {
                    Url = $"/{languages.First(l => l.Id == translation.LanguageId).Code}/projects/category/{translation.Slug}",
                    LastModified = category.UpdatedAt ?? category.CreatedAt,
                    ChangeFrequency = "weekly",
                    Priority = 0.6,
                    Alternates = alternates
                });
            }
        }

        // News Categories
        var newsCategories = await _unitOfWork.NewsCategories.GetAllAsync(c => c.IsActive, q => q.OrderBy(c => c.DisplayOrder), c => c.Translations);
        foreach (var category in newsCategories)
        {
            foreach (var translation in category.Translations)
            {
                var alternates = category.Translations
                    .Where(t => t.LanguageId != translation.LanguageId)
                    .ToDictionary(t => languages.First(l => l.Id == t.LanguageId).Code, t => $"/{languages.First(l => l.Id == t.LanguageId).Code}/news/category/{t.Slug}");
                
                urls.Add(new SitemapUrlDto
                {
                    Url = $"/{languages.First(l => l.Id == translation.LanguageId).Code}/news/category/{translation.Slug}",
                    LastModified = category.UpdatedAt ?? category.CreatedAt,
                    ChangeFrequency = "weekly",
                    Priority = 0.6,
                    Alternates = alternates
                });
            }
        }

        return urls;
    }
}