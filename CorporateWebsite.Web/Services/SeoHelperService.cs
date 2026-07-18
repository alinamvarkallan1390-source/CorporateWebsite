using CorporateWebsite.Web.Services;
using CorporateWebsite.Core.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace CorporateWebsite.Web.Services;

public interface ISeoHelperService
{
    void SetTitle(string title);
    void SetDescription(string description);
    void SetKeywords(string keywords);
    void SetCanonicalUrl(string url);
    void SetNoIndex(bool noIndex = true);
    void SetNoFollow(bool noFollow = true);
    void SetOgTitle(string title);
    void SetOgDescription(string description);
    void SetOgImage(string imageUrl);
    void SetOgType(string type);
    void SetOgUrl(string url);
    void SetTwitterCard(string card);
    void SetTwitterTitle(string title);
    void SetTwitterDescription(string description);
    void SetTwitterImage(string imageUrl);
    void SetSchemaJson(string schemaJson);
    void AddSchemaJson(string schemaJson);
    SeoDataDto GetSeoData();
    IHtmlContent RenderMetaTags();
    IHtmlContent RenderJsonLd();
    IHtmlContent RenderHreflangLinks(Dictionary<string, string> urls);
    IHtmlContent RenderCanonicalLink();
}

public class SeoHelperService : ISeoHelperService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILocalizationService _localizationService;
    private readonly ISettingService _settingService;
    private readonly SeoDataDto _seoData = new SeoDataDto();
    private readonly List<string> _schemaJsonList = new();

    public SeoHelperService(IHttpContextAccessor httpContextAccessor, ILocalizationService localizationService, ISettingService settingService)
    {
        _httpContextAccessor = httpContextAccessor;
        _localizationService = localizationService;
        _settingService = settingService;
    }

    public void SetTitle(string title) => _seoData.Title = title;
    public void SetDescription(string description) => _seoData.Description = description;
    public void SetKeywords(string keywords) => _seoData.Keywords = keywords;
    public void SetCanonicalUrl(string url) => _seoData.CanonicalUrl = url;
    public void SetNoIndex(bool noIndex = true) => _seoData.NoIndex = noIndex;
    public void SetNoFollow(bool noFollow = true) => _seoData.NoFollow = noFollow;
    public void SetOgTitle(string title) => _seoData.OgTitle = title;
    public void SetOgDescription(string description) => _seoData.OgDescription = description;
    public void SetOgImage(string imageUrl) => _seoData.OgImage = imageUrl;
    public void SetOgType(string type) => _seoData.OgType = type;
    public void SetOgUrl(string url) => _seoData.OgUrl = url;
    public void SetTwitterCard(string card) => _seoData.TwitterCard = card;
    public void SetTwitterTitle(string title) => _seoData.TwitterTitle = title;
    public void SetTwitterDescription(string description) => _seoData.TwitterDescription = description;
    public void SetTwitterImage(string imageUrl) => _seoData.TwitterImage = imageUrl;
    public void SetSchemaJson(string schemaJson)
    {
        _schemaJsonList.Clear();
        _schemaJsonList.Add(schemaJson);
    }
    public void AddSchemaJson(string schemaJson) => _schemaJsonList.Add(schemaJson);

    public SeoDataDto GetSeoData()
    {
        // Set defaults if not set
        if (string.IsNullOrEmpty(_seoData.Title))
        {
            _seoData.Title = _settingService.GetValueAsync("DefaultMetaTitle", "Corporate Website").GetAwaiter().GetResult() ?? "Corporate Website";
        }
        
        if (string.IsNullOrEmpty(_seoData.Description))
        {
            _seoData.Description = _settingService.GetValueAsync("DefaultMetaDescription", "").GetAwaiter().GetResult() ?? "";
        }
        
        if (string.IsNullOrEmpty(_seoData.Keywords))
        {
            _seoData.Keywords = _settingService.GetValueAsync("DefaultMetaKeywords", "").GetAwaiter().GetResult() ?? "";
        }

        if (string.IsNullOrEmpty(_seoData.OgImage))
        {
            _seoData.OgImage = _settingService.GetValueAsync("DefaultOgImage", "").GetAwaiter().GetResult() ?? "";
        }

        if (string.IsNullOrEmpty(_seoData.OgType))
        {
            _seoData.OgType = "website";
        }

        if (string.IsNullOrEmpty(_seoData.TwitterCard))
        {
            _seoData.TwitterCard = "summary_large_image";
        }

        return _seoData;
    }

    public IHtmlContent RenderMetaTags()
    {
        var seoData = GetSeoData();
        var sb = new StringBuilder();

        // Basic meta tags
        sb.AppendLine($"<title>{HtmlEncode(seoData.Title)}</title>");
        sb.AppendLine($"<meta name=\"description\" content=\"{HtmlEncode(seoData.Description)}\" />");
        
        if (!string.IsNullOrEmpty(seoData.Keywords))
        {
            sb.AppendLine($"<meta name=\"keywords\" content=\"{HtmlEncode(seoData.Keywords)}\" />");
        }

        // Robots
        var robots = new List<string>();
        if (seoData.NoIndex) robots.Add("noindex");
        if (seoData.NoFollow) robots.Add("nofollow");
        if (robots.Any())
        {
            sb.AppendLine($"<meta name=\"robots\" content=\"{string.Join(", ", robots)}\" />");
        }

        // Canonical
        if (!string.IsNullOrEmpty(seoData.CanonicalUrl))
        {
            sb.AppendLine($"<link rel=\"canonical\" href=\"{HtmlEncode(seoData.CanonicalUrl)}\" />");
        }

        // Open Graph
        sb.AppendLine($"<meta property=\"og:type\" content=\"{HtmlEncode(seoData.OgType)}\" />");
        sb.AppendLine($"<meta property=\"og:title\" content=\"{HtmlEncode(seoData.OgTitle ?? seoData.Title)}\" />");
        sb.AppendLine($"<meta property=\"og:description\" content=\"{HtmlEncode(seoData.OgDescription ?? seoData.Description)}\" />");
        
        if (!string.IsNullOrEmpty(seoData.OgImage))
        {
            var ogImage = seoData.OgImage.StartsWith("http") ? seoData.OgImage : GetAbsoluteUrl(seoData.OgImage);
            sb.AppendLine($"<meta property=\"og:image\" content=\"{HtmlEncode(ogImage)}\" />");
        }

        if (!string.IsNullOrEmpty(seoData.OgUrl))
        {
            sb.AppendLine($"<meta property=\"og:url\" content=\"{HtmlEncode(seoData.OgUrl)}\" />");
        }

        // Twitter Card
        sb.AppendLine($"<meta name=\"twitter:card\" content=\"{HtmlEncode(seoData.TwitterCard)}\" />");
        sb.AppendLine($"<meta name=\"twitter:title\" content=\"{HtmlEncode(seoData.TwitterTitle ?? seoData.Title)}\" />");
        sb.AppendLine($"<meta name=\"twitter:description\" content=\"{HtmlEncode(seoData.TwitterDescription ?? seoData.Description)}\" />");
        
        if (!string.IsNullOrEmpty(seoData.TwitterImage))
        {
            var twitterImage = seoData.TwitterImage.StartsWith("http") ? seoData.TwitterImage : GetAbsoluteUrl(seoData.TwitterImage);
            sb.AppendLine($"<meta name=\"twitter:image\" content=\"{HtmlEncode(twitterImage)}\" />");
        }

        // Viewport and theme color
        sb.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" />");
        sb.AppendLine("<meta name=\"theme-color\" content=\"#2563eb\" />");

        return new HtmlString(sb.ToString());
    }

    public IHtmlContent RenderJsonLd()
    {
        var sb = new StringBuilder();
        
        foreach (var schema in _schemaJsonList)
        {
            sb.AppendLine($"<script type=\"application/ld+json\">{schema}</script>");
        }

        return new HtmlString(sb.ToString());
    }

    public IHtmlContent RenderHreflangLinks(Dictionary<string, string> urls)
    {
        if (!urls.Any()) return new HtmlString("");

        var sb = new StringBuilder();
        
        foreach (var url in urls)
        {
            sb.AppendLine($"<link rel=\"alternate\" hreflang=\"{url.Key}\" href=\"{HtmlEncode(url.Value)}\" />");
        }
        
        // x-default
        var defaultUrl = urls.FirstOrDefault().Value;
        sb.AppendLine($"<link rel=\"alternate\" hreflang=\"x-default\" href=\"{HtmlEncode(defaultUrl)}\" />");

        return new HtmlString(sb.ToString());
    }

    public IHtmlContent RenderCanonicalLink()
    {
        var seoData = GetSeoData();
        if (string.IsNullOrEmpty(seoData.CanonicalUrl))
        {
            return new HtmlString("");
        }

        return new HtmlString($"<link rel=\"canonical\" href=\"{HtmlEncode(seoData.CanonicalUrl)}\" />");
    }

    private string GetAbsoluteUrl(string relativeUrl)
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        if (request == null) return relativeUrl;
        
        return $"{request.Scheme}://{request.Host}{relativeUrl}";
    }

    private string HtmlEncode(string value)
    {
        return System.Net.WebUtility.HtmlEncode(value);
    }
}