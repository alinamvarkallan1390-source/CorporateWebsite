namespace CorporateWebsite.Core.DTOs;

public class SeoDataDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Keywords { get; set; }
    public string? CanonicalUrl { get; set; }
    public bool NoIndex { get; set; }
    public bool NoFollow { get; set; }
    public string? OgType { get; set; }
    public string? OgTitle { get; set; }
    public string? OgDescription { get; set; }
    public string? OgImage { get; set; }
    public string? OgUrl { get; set; }
    public string? TwitterCard { get; set; }
    public string? TwitterTitle { get; set; }
    public string? TwitterDescription { get; set; }
    public string? TwitterImage { get; set; }
    public string? SchemaJson { get; set; }
    public Dictionary<string, string> HreflangUrls { get; set; } = new Dictionary<string, string>();
    public ICollection<BreadcrumbDto> Breadcrumbs { get; set; } = new List<BreadcrumbDto>();
}

public class SeoValidationDto
{
    public string Url { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Keywords { get; set; }
    public string? H1 { get; set; }
    public ICollection<string> Headings { get; set; } = new List<string>();
    public ICollection<string> Images { get; set; } = new List<string>();
    public int WordCount { get; set; }
    public int LinkCount { get; set; }
    public bool HasCanonical { get; set; }
    public bool HasOgTags { get; set; }
    public bool HasTwitterCards { get; set; }
    public bool HasSchema { get; set; }
    public bool HasHreflang { get; set; }
    public bool IsIndexable { get; set; }
    public bool IsCrawlable { get; set; }
}

public class SeoAuditDto
{
    public string Url { get; set; } = string.Empty;
    public int Score { get; set; }
    public SeoAuditCategoryDto Technical { get; set; } = new SeoAuditCategoryDto();
    public SeoAuditCategoryDto Content { get; set; } = new SeoAuditCategoryDto();
    public SeoAuditCategoryDto Performance { get; set; } = new SeoAuditCategoryDto();
    public SeoAuditCategoryDto Mobile { get; set; } = new SeoAuditCategoryDto();
    public ICollection<SeoIssueDto> Issues { get; set; } = new List<SeoIssueDto>();
    public DateTime AuditedAt { get; set; } = DateTime.UtcNow;
}

public class SeoAuditCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public int Score { get; set; }
    public ICollection<SeoCheckDto> Checks { get; set; } = new List<SeoCheckDto>();
}

public class SeoCheckDto
{
    public string Name { get; set; } = string.Empty;
    public bool Passed { get; set; }
    public string? Message { get; set; }
    public SeoSeverity Severity { get; set; }
}

public class SeoIssueDto
{
    public string Type { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public SeoSeverity Severity { get; set; }
    public string? Recommendation { get; set; }
}

public enum SeoSeverity
{
    Info,
    Warning,
    Error,
    Critical
}

public class SearchConsoleErrorDto
{
    public string Url { get; set; } = string.Empty;
    public string ErrorType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DetectedAt { get; set; }
    public bool IsFixed { get; set; }
    public DateTime? FixedAt { get; set; }
}

public class SearchResultDto
{
    public string Query { get; set; } = string.Empty;
    public int TotalResults { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public double SearchTimeMs { get; set; }
    public ICollection<SearchResultItemDto> Items { get; set; } = new List<SearchResultItemDto>();
}

public class SearchResultItemDto
{
    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public DateTime? PublishedAt { get; set; }
    public double Score { get; set; }
    public Dictionary<string, string> Highlights { get; set; } = new Dictionary<string, string>();
}

public class SearchSuggestionDto
{
    public string Text { get; set; } = string.Empty;
    public int Count { get; set; }
    public string Type { get; set; } = string.Empty; // query, page, service, project, news
}

public class SitemapUrlDto
{
    public string Url { get; set; } = string.Empty;
    public DateTime? LastModified { get; set; }
    public string? ChangeFrequency { get; set; } // always, hourly, daily, weekly, monthly, yearly, never
    public double? Priority { get; set; } // 0.0 to 1.0
    public Dictionary<string, string> Alternates { get; set; } = new Dictionary<string, string>(); // hreflang
}

public class RobotsTxtDto
{
    public string UserAgent { get; set; } = "*";
    public ICollection<string> Disallow { get; set; } = new List<string>();
    public ICollection<string> Allow { get; set; } = new List<string>();
    public string? Sitemap { get; set; }
    public ICollection<string> Host { get; set; } = new List<string>();
    public int? CrawlDelay { get; set; }
}