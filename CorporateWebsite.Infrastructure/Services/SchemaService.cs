using CorporateWebsite.Core.DTOs;
using CorporateWebsite.Core.Entities;
using CorporateWebsite.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CorporateWebsite.Infrastructure.Services;

public interface ISchemaService
{
    Task<string> GenerateOrganizationSchemaAsync();
    Task<string> GenerateWebSiteSchemaAsync();
    Task<string> GenerateWebPageSchemaAsync(string url, string name, string? description, string? image);
    Task<string> GenerateArticleSchemaAsync(NewsDto news, string languageCode);
    Task<string> GenerateNewsArticleSchemaAsync(NewsDto news, string languageCode);
    Task<string> GenerateServiceSchemaAsync(ServiceDto service, string languageCode);
    Task<string> GenerateFaqPageSchemaAsync(ServiceDto service, string languageCode);
    Task<string> GenerateBreadcrumbSchemaAsync(IReadOnlyList<BreadcrumbDto> breadcrumbs);
    Task<string> GeneratePersonSchemaAsync(ApplicationUser user);
    Task<string> GenerateVideoObjectSchemaAsync(string name, string description, string url, string thumbnailUrl, DateTime uploadDate);
    Task<string> GenerateCustomSchemaAsync(string schemaType, object data);
}

public class SchemaService : ISchemaService
{
    private readonly ISettingService _settingService;
    private readonly ILanguageService _languageService;

    public SchemaService(ISettingService settingService, ILanguageService languageService)
    {
        _settingService = settingService;
        _languageService = languageService;
    }

    public async Task<string> GenerateOrganizationSchemaAsync()
    {
        var companyName = await _settingService.GetValueAsync("CompanyName", "Corporate Website");
        var companyUrl = await _settingService.GetValueAsync("CanonicalDomain", "https://localhost");
        var logo = await _settingService.GetValueAsync("SiteLogo", "");
        var address = await _settingService.GetValueAsync("CompanyAddress", "");
        var phone = await _settingService.GetValueAsync("CompanyPhone", "");
        var email = await _settingService.GetValueAsync("CompanyEmail", "");
        
        var sameAs = new List<string>();
        var facebook = await _settingService.GetValueAsync("FacebookUrl", "");
        var twitter = await _settingService.GetValueAsync("TwitterUrl", "");
        var linkedin = await _settingService.GetValueAsync("LinkedInUrl", "");
        var instagram = await _settingService.GetValueAsync("InstagramUrl", "");
        var youtube = await _settingService.GetValueAsync("YouTubeUrl", "");
        
        if (!string.IsNullOrEmpty(facebook)) sameAs.Add(facebook);
        if (!string.IsNullOrEmpty(twitter)) sameAs.Add(twitter);
        if (!string.IsNullOrEmpty(linkedin)) sameAs.Add(linkedin);
        if (!string.IsNullOrEmpty(instagram)) sameAs.Add(instagram);
        if (!string.IsNullOrEmpty(youtube)) sameAs.Add(youtube);

        var schema = new
        {
            @context = "https://schema.org",
            @type = "Organization",
            name = companyName,
            url = companyUrl,
            logo = string.IsNullOrEmpty(logo) ? null : $"{companyUrl.TrimEnd('/')}{logo}",
            address = string.IsNullOrEmpty(address) ? null : new
            {
                @type = "PostalAddress",
                streetAddress = address
            },
            telephone = string.IsNullOrEmpty(phone) ? null : phone,
            email = string.IsNullOrEmpty(email) ? null : email,
            sameAs = sameAs.Any() ? sameAs : null
        };

        return JsonSerializer.Serialize(schema, new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });
    }

    public async Task<string> GenerateWebSiteSchemaAsync()
    {
        var siteName = await _settingService.GetValueAsync("SiteName", "Corporate Website");
        var siteUrl = await _settingService.GetValueAsync("CanonicalDomain", "https://localhost");
        
        var schema = new
        {
            @context = "https://schema.org",
            @type = "WebSite",
            name = siteName,
            url = siteUrl,
            potentialAction = new
            {
                @type = "SearchAction",
                target = new
                {
                    @type = "EntryPoint",
                    urlTemplate = $"{siteUrl.TrimEnd('/')}/search?q={{search_term_string}}"
                },
                queryInput = "required name=search_term_string"
            }
        };

        return JsonSerializer.Serialize(schema, new JsonSerializerOptions { WriteIndented = true });
    }

    public async Task<string> GenerateWebPageSchemaAsync(string url, string name, string? description, string? image)
    {
        var siteUrl = await _settingService.GetValueAsync("CanonicalDomain", "https://localhost");
        
        var schema = new
        {
            @context = "https://schema.org",
            @type = "WebPage",
            name = name,
            description = description,
            url = url.StartsWith("http") ? url : $"{siteUrl.TrimEnd('/')}{url}",
            image = string.IsNullOrEmpty(image) ? null : (image.StartsWith("http") ? image : $"{siteUrl.TrimEnd('/')}{image}"),
            isPartOf = new
            {
                @type = "WebSite",
                name = await _settingService.GetValueAsync("SiteName", "Corporate Website"),
                url = siteUrl
            }
        };

        return JsonSerializer.Serialize(schema, new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });
    }

    public async Task<string> GenerateArticleSchemaAsync(NewsDto news, string languageCode)
    {
        var translation = news.Translation ?? news.Translations.FirstOrDefault();
        if (translation == null) return "{}";

        var siteUrl = await _settingService.GetValueAsync("CanonicalDomain", "https://localhost");
        var authorName = news.AuthorName ?? "Corporate Website Team";
        
        var schema = new
        {
            @context = "https://schema.org",
            @type = "Article",
            headline = translation.Title,
            description = translation.MetaDescription ?? translation.ShortDescription,
            image = news.MainImageUrl != null ? $"{siteUrl.TrimEnd('/')}{news.MainImageUrl}" : null,
            datePublished = news.PublishedAt?.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            dateModified = news.UpdatedAt?.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            author = new
            {
                @type = "Person",
                name = authorName
            },
            publisher = new
            {
                @type = "Organization",
                name = await _settingService.GetValueAsync("CompanyName", "Corporate Website"),
                logo = new
                {
                    @type = "ImageObject",
                    url = string.IsNullOrEmpty(await _settingService.GetValueAsync("SiteLogo", "")) ? null : $"{siteUrl.TrimEnd('/')}{await _settingService.GetValueAsync("SiteLogo", "")}"
                }
            },
            mainEntityOfPage = new
            {
                @type = "WebPage",
                @id = $"{siteUrl.TrimEnd('/')}/{languageCode}/news/{translation.Slug}"
            },
            inLanguage = languageCode
        };

        return JsonSerializer.Serialize(schema, new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });
    }

    public async Task<string> GenerateNewsArticleSchemaAsync(NewsDto news, string languageCode)
    {
        var translation = news.Translation ?? news.Translations.FirstOrDefault();
        if (translation == null) return "{}";

        var siteUrl = await _settingService.GetValueAsync("CanonicalDomain", "https://localhost");
        var authorName = news.AuthorName ?? "Corporate Website Team";
        
        var schema = new
        {
            @context = "https://schema.org",
            @type = "NewsArticle",
            headline = translation.Title,
            description = translation.MetaDescription ?? translation.ShortDescription,
            image = news.MainImageUrl != null ? $"{siteUrl.TrimEnd('/')}{news.MainImageUrl}" : null,
            datePublished = news.PublishedAt?.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            dateModified = news.UpdatedAt?.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            author = new
            {
                @type = "Person",
                name = authorName
            },
            publisher = new
            {
                @type = "Organization",
                name = await _settingService.GetValueAsync("CompanyName", "Corporate Website"),
                logo = new
                {
                    @type = "ImageObject",
                    url = string.IsNullOrEmpty(await _settingService.GetValueAsync("SiteLogo", "")) ? null : $"{siteUrl.TrimEnd('/')}{await _settingService.GetValueAsync("SiteLogo", "")}"
                }
            },
            mainEntityOfPage = new
            {
                @type = "WebPage",
                @id = $"{siteUrl.TrimEnd('/')}/{languageCode}/news/{translation.Slug}"
            },
            inLanguage = languageCode,
            articleSection = news.CategoryName,
            keywords = translation.MetaKeywords
        };

        return JsonSerializer.Serialize(schema, new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });
    }

    public async Task<string> GenerateServiceSchemaAsync(ServiceDto service, string languageCode)
    {
        var translation = service.Translation ?? service.Translations.FirstOrDefault();
        if (translation == null) return "{}";

        var siteUrl = await _settingService.GetValueAsync("CanonicalDomain", "https://localhost");
        
        var schema = new
        {
            @context = "https://schema.org",
            @type = "Service",
            name = translation.Title,
            description = translation.MetaDescription ?? translation.ShortDescription,
            image = service.ImageUrl != null ? $"{siteUrl.TrimEnd('/')}{service.ImageUrl}" : null,
            provider = new
            {
                @type = "Organization",
                name = await _settingService.GetValueAsync("CompanyName", "Corporate Website"),
                url = siteUrl
            },
            serviceType = service.CategoryName,
            areaServed = new
            {
                @type = "Place",
                name = "Global"
            },
            availableChannel = new
            {
                @type = "ServiceChannel",
                serviceUrl = $"{siteUrl.TrimEnd('/')}/{languageCode}/services/{translation.Slug}"
            }
        };

        return JsonSerializer.Serialize(schema, new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });
    }

    public async Task<string> GenerateFaqPageSchemaAsync(ServiceDto service, string languageCode)
    {
        var translation = service.Translation ?? service.Translations.FirstOrDefault();
        if (translation == null) return "{}";

        var mainEntity = service.Faqs
            .Where(f => f.Translation != null)
            .Select(f => new
            {
                @type = "Question",
                name = f.Translation.Question,
                acceptedAnswer = new
                {
                    @type = "Answer",
                    text = f.Translation.Answer
                }
            })
            .ToList();

        var schema = new
        {
            @context = "https://schema.org",
            @type = "FAQPage",
            mainEntity = mainEntity
        };

        return JsonSerializer.Serialize(schema, new JsonSerializerOptions { WriteIndented = true });
    }

    public async Task<string> GenerateBreadcrumbSchemaAsync(IReadOnlyList<BreadcrumbDto> breadcrumbs)
    {
        var itemListElements = breadcrumbs
            .Select((b, index) => new
            {
                @type = "ListItem",
                position = index + 1,
                name = b.Title,
                item = b.Url
            })
            .ToList();

        var schema = new
        {
            @context = "https://schema.org",
            @type = "BreadcrumbList",
            itemListElement = itemListElements
        };

        return JsonSerializer.Serialize(schema, new JsonSerializerOptions { WriteIndented = true });
    }

    public async Task<string> GeneratePersonSchemaAsync(ApplicationUser user)
    {
        var siteUrl = await _settingService.GetValueAsync("CanonicalDomain", "https://localhost");
        
        var schema = new
        {
            @context = "https://schema.org",
            @type = "Person",
            name = $"{user.FirstName} {user.LastName}".Trim(),
            jobTitle = "Team Member",
            url = string.IsNullOrEmpty(user.AvatarUrl) ? null : $"{siteUrl.TrimEnd('/')}{user.AvatarUrl}",
            image = string.IsNullOrEmpty(user.AvatarUrl) ? null : $"{siteUrl.TrimEnd('/')}{user.AvatarUrl}",
            description = user.Bio,
            worksFor = new
            {
                @type = "Organization",
                name = await _settingService.GetValueAsync("CompanyName", "Corporate Website"),
                url = siteUrl
            }
        };

        return JsonSerializer.Serialize(schema, new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });
    }

    public async Task<string> GenerateVideoObjectSchemaAsync(string name, string description, string url, string thumbnailUrl, DateTime uploadDate)
    {
        var siteUrl = await _settingService.GetValueAsync("CanonicalDomain", "https://localhost");
        
        var schema = new
        {
            @context = "https://schema.org",
            @type = "VideoObject",
            name = name,
            description = description,
            contentUrl = url.StartsWith("http") ? url : $"{siteUrl.TrimEnd('/')}{url}",
            thumbnailUrl = thumbnailUrl.StartsWith("http") ? thumbnailUrl : $"{siteUrl.TrimEnd('/')}{thumbnailUrl}",
            uploadDate = uploadDate.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            publisher = new
            {
                @type = "Organization",
                name = await _settingService.GetValueAsync("CompanyName", "Corporate Website"),
                logo = new
                {
                    @type = "ImageObject",
                    url = string.IsNullOrEmpty(await _settingService.GetValueAsync("SiteLogo", "")) ? null : $"{siteUrl.TrimEnd('/')}{await _settingService.GetValueAsync("SiteLogo", "")}"
                }
            }
        };

        return JsonSerializer.Serialize(schema, new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });
    }

    public async Task<string> GenerateCustomSchemaAsync(string schemaType, object data)
    {
        var schema = new
        {
            @context = "https://schema.org",
            @type = schemaType
        };

        // Merge data with base schema
        var jsonData = JsonSerializer.Serialize(data);
        var baseSchema = JsonSerializer.Serialize(schema);
        
        // This is a simplified merge - in production you'd want a proper JSON merge
        var merged = JsonSerializer.Deserialize<Dictionary<string, object>>(baseSchema);
        var dataDict = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonData);
        
        foreach (var kvp in dataDict)
        {
            merged[kvp.Key] = kvp.Value;
        }

        return JsonSerializer.Serialize(merged, new JsonSerializerOptions { WriteIndented = true });
    }
}