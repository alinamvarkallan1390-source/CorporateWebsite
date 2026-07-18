using CorporateWebsite.Web.Services;
using CorporateWebsite.Core.DTOs;
using CorporateWebsite.Core.Entities;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace CorporateWebsite.Web.Services;

public interface ISchemaHelperService
{
    string GenerateOrganizationSchema();
    string GenerateWebSiteSchema();
    string GenerateWebPageSchema(string url, string name, string? description, string? image);
    string GenerateArticleSchema(NewsDto news, string languageCode);
    string GenerateNewsArticleSchema(NewsDto news, string languageCode);
    string GenerateServiceSchema(ServiceDto service, string languageCode);
    string GenerateFaqPageSchema(ServiceDto service, string languageCode);
    string GenerateBreadcrumbSchema(IReadOnlyList<BreadcrumbDto> breadcrumbs);
    string GeneratePersonSchema(ApplicationUser user);
    string GenerateVideoObjectSchema(string name, string description, string url, string thumbnailUrl, DateTime uploadDate);
    string GenerateProductSchema(string name, string description, string image, string url, decimal price, string currency, string availability);
    string GenerateCustomSchema(string schemaType, object data);
}

public class SchemaHelperService : ISchemaHelperService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ISettingService _settingService;

    public SchemaHelperService(IHttpContextAccessor httpContextAccessor, ISettingService settingService)
    {
        _httpContextAccessor = httpContextAccessor;
        _settingService = settingService;
    }

    private string GetAbsoluteUrl(string relativeUrl)
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        if (request == null) return relativeUrl;
        if (relativeUrl.StartsWith("http")) return relativeUrl;
        return $"{request.Scheme}://{request.Host}{relativeUrl}";
    }

    public string GenerateOrganizationSchema()
    {
        var companyName = _settingService.GetValueAsync("CompanyName", "Corporate Website").GetAwaiter().GetResult();
        var companyUrl = _settingService.GetValueAsync("CanonicalDomain", "https://localhost").GetAwaiter().GetResult();
        var logo = _settingService.GetValueAsync("SiteLogo", "").GetAwaiter().GetResult();
        var address = _settingService.GetValueAsync("CompanyAddress", "").GetAwaiter().GetResult();
        var phone = _settingService.GetValueAsync("CompanyPhone", "").GetAwaiter().GetResult();
        var email = _settingService.GetValueAsync("CompanyEmail", "").GetAwaiter().GetResult();
        
        var sameAs = new List<string>();
        var facebook = _settingService.GetValueAsync("FacebookUrl", "").GetAwaiter().GetResult();
        var twitter = _settingService.GetValueAsync("TwitterUrl", "").GetAwaiter().GetResult();
        var linkedin = _settingService.GetValueAsync("LinkedInUrl", "").GetAwaiter().GetResult();
        var instagram = _settingService.GetValueAsync("InstagramUrl", "").GetAwaiter().GetResult();
        var youtube = _settingService.GetValueAsync("YouTubeUrl", "").GetAwaiter().GetResult();
        
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
            logo = string.IsNullOrEmpty(logo) ? null : GetAbsoluteUrl(logo),
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

    public string GenerateWebSiteSchema()
    {
        var siteName = _settingService.GetValueAsync("SiteName", "Corporate Website").GetAwaiter().GetResult();
        var siteUrl = _settingService.GetValueAsync("CanonicalDomain", "https://localhost").GetAwaiter().GetResult();
        
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

    public string GenerateWebPageSchema(string url, string name, string? description, string? image)
    {
        var siteUrl = _settingService.GetValueAsync("CanonicalDomain", "https://localhost").GetAwaiter().GetResult();
        
        var schema = new
        {
            @context = "https://schema.org",
            @type = "WebPage",
            name = name,
            description = description,
            url = GetAbsoluteUrl(url),
            image = string.IsNullOrEmpty(image) ? null : GetAbsoluteUrl(image),
            isPartOf = new
            {
                @type = "WebSite",
                name = _settingService.GetValueAsync("SiteName", "Corporate Website").GetAwaiter().GetResult(),
                url = siteUrl
            }
        };

        return JsonSerializer.Serialize(schema, new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });
    }

    public string GenerateArticleSchema(NewsDto news, string languageCode)
    {
        var translation = news.Translation ?? news.Translations.FirstOrDefault();
        if (translation == null) return "{}";

        var siteUrl = _settingService.GetValueAsync("CanonicalDomain", "https://localhost").GetAwaiter().GetResult();
        var authorName = news.AuthorName ?? "Corporate Website Team";
        
        var schema = new
        {
            @context = "https://schema.org",
            @type = "Article",
            headline = translation.Title,
            description = translation.MetaDescription ?? translation.ShortDescription,
            image = news.MainImageUrl != null ? GetAbsoluteUrl(news.MainImageUrl) : null,
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
                name = _settingService.GetValueAsync("CompanyName", "Corporate Website").GetAwaiter().GetResult(),
                logo = new
                {
                    @type = "ImageObject",
                    url = string.IsNullOrEmpty(_settingService.GetValueAsync("SiteLogo", "").GetAwaiter().GetResult()) ? null : GetAbsoluteUrl(_settingService.GetValueAsync("SiteLogo", "").GetAwaiter().GetResult())
                }
            },
            mainEntityOfPage = new
            {
                @type = "WebPage",
                @id = GetAbsoluteUrl($"/{languageCode}/news/{translation.Slug}")
            },
            inLanguage = languageCode
        };

        return JsonSerializer.Serialize(schema, new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });
    }

    public string GenerateNewsArticleSchema(NewsDto news, string languageCode)
    {
        var translation = news.Translation ?? news.Translations.FirstOrDefault();
        if (translation == null) return "{}";

        var siteUrl = _settingService.GetValueAsync("CanonicalDomain", "https://localhost").GetAwaiter().GetResult();
        var authorName = news.AuthorName ?? "Corporate Website Team";
        
        var schema = new
        {
            @context = "https://schema.org",
            @type = "NewsArticle",
            headline = translation.Title,
            description = translation.MetaDescription ?? translation.ShortDescription,
            image = news.MainImageUrl != null ? GetAbsoluteUrl(news.MainImageUrl) : null,
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
                name = _settingService.GetValueAsync("CompanyName", "Corporate Website").GetAwaiter().GetResult(),
                logo = new
                {
                    @type = "ImageObject",
                    url = string.IsNullOrEmpty(_settingService.GetValueAsync("SiteLogo", "").GetAwaiter().GetResult()) ? null : GetAbsoluteUrl(_settingService.GetValueAsync("SiteLogo", "").GetAwaiter().GetResult())
                }
            },
            mainEntityOfPage = new
            {
                @type = "WebPage",
                @id = GetAbsoluteUrl($"/{languageCode}/news/{translation.Slug}")
            },
            inLanguage = languageCode,
            articleSection = news.CategoryName,
            keywords = translation.MetaKeywords
        };

        return JsonSerializer.Serialize(schema, new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });
    }

    public string GenerateServiceSchema(ServiceDto service, string languageCode)
    {
        var translation = service.Translation ?? service.Translations.FirstOrDefault();
        if (translation == null) return "{}";

        var siteUrl = _settingService.GetValueAsync("CanonicalDomain", "https://localhost").GetAwaiter().GetResult();
        
        var schema = new
        {
            @context = "https://schema.org",
            @type = "Service",
            name = translation.Title,
            description = translation.MetaDescription ?? translation.ShortDescription,
            image = service.ImageUrl != null ? GetAbsoluteUrl(service.ImageUrl) : null,
            provider = new
            {
                @type = "Organization",
                name = _settingService.GetValueAsync("CompanyName", "Corporate Website").GetAwaiter().GetResult(),
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
                serviceUrl = GetAbsoluteUrl($"/{languageCode}/services/{translation.Slug}")
            }
        };

        return JsonSerializer.Serialize(schema, new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });
    }

    public string GenerateFaqPageSchema(ServiceDto service, string languageCode)
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

    public string GenerateBreadcrumbSchema(IReadOnlyList<BreadcrumbDto> breadcrumbs)
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

    public string GeneratePersonSchema(ApplicationUser user)
    {
        var siteUrl = _settingService.GetValueAsync("CanonicalDomain", "https://localhost").GetAwaiter().GetResult();
        
        var schema = new
        {
            @context = "https://schema.org",
            @type = "Person",
            name = $"{user.FirstName} {user.LastName}".Trim(),
            jobTitle = "Team Member",
            url = string.IsNullOrEmpty(user.AvatarUrl) ? null : GetAbsoluteUrl(user.AvatarUrl),
            image = string.IsNullOrEmpty(user.AvatarUrl) ? null : GetAbsoluteUrl(user.AvatarUrl),
            description = user.Bio,
            worksFor = new
            {
                @type = "Organization",
                name = _settingService.GetValueAsync("CompanyName", "Corporate Website").GetAwaiter().GetResult(),
                url = siteUrl
            }
        };

        return JsonSerializer.Serialize(schema, new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });
    }

    public string GenerateVideoObjectSchema(string name, string description, string url, string thumbnailUrl, DateTime uploadDate)
    {
        var siteUrl = _settingService.GetValueAsync("CanonicalDomain", "https://localhost").GetAwaiter().GetResult();
        
        var schema = new
        {
            @context = "https://schema.org",
            @type = "VideoObject",
            name = name,
            description = description,
            contentUrl = GetAbsoluteUrl(url),
            thumbnailUrl = GetAbsoluteUrl(thumbnailUrl),
            uploadDate = uploadDate.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            publisher = new
            {
                @type = "Organization",
                name = _settingService.GetValueAsync("CompanyName", "Corporate Website").GetAwaiter().GetResult(),
                logo = new
                {
                    @type = "ImageObject",
                    url = string.IsNullOrEmpty(_settingService.GetValueAsync("SiteLogo", "").GetAwaiter().GetResult()) ? null : GetAbsoluteUrl(_settingService.GetValueAsync("SiteLogo", "").GetAwaiter().GetResult())
                }
            }
        };

        return JsonSerializer.Serialize(schema, new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });
    }

    public string GenerateProductSchema(string name, string description, string image, string url, decimal price, string currency, string availability)
    {
        var siteUrl = _settingService.GetValueAsync("CanonicalDomain", "https://localhost").GetAwaiter().GetResult();
        
        var schema = new
        {
            @context = "https://schema.org",
            @type = "Product",
            name = name,
            description = description,
            image = GetAbsoluteUrl(image),
            url = GetAbsoluteUrl(url),
            offers = new
            {
                @type = "Offer",
                price = price.ToString("F2"),
                priceCurrency = currency,
                availability = $"https://schema.org/{availability}",
                seller = new
                {
                    @type = "Organization",
                    name = _settingService.GetValueAsync("CompanyName", "Corporate Website").GetAwaiter().GetResult()
                }
            }
        };

        return JsonSerializer.Serialize(schema, new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });
    }

    public string GenerateCustomSchema(string schemaType, object data)
    {
        var baseSchema = new
        {
            @context = "https://schema.org",
            @type = schemaType
        };

        var jsonData = JsonSerializer.Serialize(data);
        var dataDict = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonData);
        var baseDict = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(baseSchema));
        
        foreach (var kvp in dataDict)
        {
            baseDict[kvp.Key] = kvp.Value;
        }

        return JsonSerializer.Serialize(baseDict, new JsonSerializerOptions { WriteIndented = true });
    }
}
