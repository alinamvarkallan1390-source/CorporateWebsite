using CorporateWebsite.Web.Services;
using CorporateWebsite.Core.DTOs;
using CorporateWebsite.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace CorporateWebsite.Web.Services;

public interface IBreadcrumbService
{
    void Add(string title, string? url = null, bool isCurrent = false);
    void AddRange(IEnumerable<BreadcrumbDto> breadcrumbs);
    void Clear();
    IReadOnlyList<BreadcrumbDto> GetBreadcrumbs();
    IHtmlContent Render();
    IHtmlContent RenderJsonLd();
}

public class BreadcrumbService : IBreadcrumbService
{
    private readonly List<BreadcrumbDto> _breadcrumbs = new();
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILocalizationService _localizationService;

    public BreadcrumbService(IHttpContextAccessor httpContextAccessor, ILocalizationService localizationService)
    {
        _httpContextAccessor = httpContextAccessor;
        _localizationService = localizationService;
    }

    public void Add(string title, string? url = null, bool isCurrent = false)
    {
        _breadcrumbs.Add(new BreadcrumbDto
        {
            Title = title,
            Url = url,
            IsCurrent = isCurrent
        });
    }

    public void AddRange(IEnumerable<BreadcrumbDto> breadcrumbs)
    {
        _breadcrumbs.AddRange(breadcrumbs);
    }

    public void Clear()
    {
        _breadcrumbs.Clear();
    }

    public IReadOnlyList<BreadcrumbDto> GetBreadcrumbs() => _breadcrumbs.AsReadOnly();

    public IHtmlContent Render()
    {
        if (!_breadcrumbs.Any()) return new HtmlString("");

        var sb = new StringBuilder();
        var culture = _localizationService.CurrentCulture;
        
        sb.AppendLine("<nav aria-label=\"Breadcrumb\" class=\"breadcrumb-nav\">");
        sb.AppendLine("  <ol class=\"breadcrumb\" itemscope itemtype=\"https://schema.org/BreadcrumbList\">");

        for (int i = 0; i < _breadcrumbs.Count; i++)
        {
            var item = _breadcrumbs[i];
            var isLast = i == _breadcrumbs.Count - 1;
            
            sb.AppendLine($"    <li class=\"breadcrumb-item{(isLast ? " active" : "")}\" itemprop=\"itemListElement\" itemscope itemtype=\"https://schema.org/ListItem\">");
            sb.AppendLine($"      <meta itemprop=\"position\" content=\"{i + 1}\" />");
            
            if (!isLast && !string.IsNullOrEmpty(item.Url))
            {
                var url = item.Url.StartsWith("/") ? $"/{culture}{item.Url}" : item.Url;
                sb.AppendLine($"      <a itemprop=\"item\" href=\"{url}\"><span itemprop=\"name\">{System.Net.WebUtility.HtmlEncode(item.Title)}</span></a>");
            }
            else
            {
                sb.AppendLine($"      <span itemprop=\"name\" aria-current=\"page\">{System.Net.WebUtility.HtmlEncode(item.Title)}</span>");
            }
            
            sb.AppendLine("    </li>");
        }

        sb.AppendLine("  </ol>");
        sb.AppendLine("</nav>");

        return new HtmlString(sb.ToString());
    }

    public IHtmlContent RenderJsonLd()
    {
        if (!_breadcrumbs.Any()) return new HtmlString("");

        var itemListElements = _breadcrumbs
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

        var json = System.Text.Json.JsonSerializer.Serialize(schema, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        return new HtmlString($"<script type=\"application/ld+json\">{json}</script>");
    }
}