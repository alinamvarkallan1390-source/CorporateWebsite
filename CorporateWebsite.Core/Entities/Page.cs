namespace CorporateWebsite.Core.Entities;

public class Page : BaseEntity
{
    public string Slug { get; set; } = string.Empty;
    public string Layout { get; set; } = "Default";
    public bool IsPublished { get; set; } = false;
    public DateTime? PublishedAt { get; set; }
    public int? ParentId { get; set; }
    public int DisplayOrder { get; set; }
    public bool ShowInMenu { get; set; } = true;
    public string? Icon { get; set; }
    public string? Template { get; set; }
    
    // SEO
    public string? CanonicalUrl { get; set; }
    public bool NoIndex { get; set; }
    public bool NoFollow { get; set; }
    public string? OgType { get; set; } = "website";
    
    // Navigation
    public Page? Parent { get; set; }
    public ICollection<Page> Children { get; set; } = new List<Page>();
    public ICollection<PageTranslation> Translations { get; set; } = new List<PageTranslation>();
    public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
    public ICollection<Form> Forms { get; set; } = new List<Form>();
}

public class PageTranslation : BaseEntity
{
    public int PageId { get; set; }
    public int LanguageId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public string? OgTitle { get; set; }
    public string? OgDescription { get; set; }
    public string? OgImage { get; set; }
    public string? TwitterCard { get; set; }
    public string? TwitterTitle { get; set; }
    public string? TwitterDescription { get; set; }
    public string? TwitterImage { get; set; }
    public string? SchemaJson { get; set; }
    
    // Navigation
    public Page Page { get; set; } = null!;
    public Language Language { get; set; } = null!;
}