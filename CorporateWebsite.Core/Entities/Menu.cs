namespace CorporateWebsite.Core.Entities;

public class Menu : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    
    public ICollection<MenuItem> Items { get; set; } = new List<MenuItem>();
    public ICollection<MenuTranslation> Translations { get; set; } = new List<MenuTranslation>();
}

public class MenuTranslation : BaseEntity
{
    public int MenuId { get; set; }
    public int LanguageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    public Menu Menu { get; set; } = null!;
    public Language Language { get; set; } = null!;
}

public class MenuItem : BaseEntity
{
    public int MenuId { get; set; }
    public int? ParentId { get; set; }
    public int? PageId { get; set; }
    public int? ServiceId { get; set; }
    public int? ProjectId { get; set; }
    public int? NewsCategoryId { get; set; }
    public string? CustomUrl { get; set; }
    public string? Target { get; set; } = "_self";
    public string? CssClass { get; set; }
    public string? Icon { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsMegaMenu { get; set; }
    public string? MegaMenuContent { get; set; }
    
    public Menu Menu { get; set; } = null!;
    public MenuItem? Parent { get; set; }
    public ICollection<MenuItem> Children { get; set; } = new List<MenuItem>();
    public Page? Page { get; set; }
    public Service? Service { get; set; }
    public Project? Project { get; set; }
    public NewsCategory? NewsCategory { get; set; }
    public ICollection<MenuItemTranslation> Translations { get; set; } = new List<MenuItemTranslation>();
}

public class MenuItemTranslation : BaseEntity
{
    public int MenuItemId { get; set; }
    public int LanguageId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Tooltip { get; set; }
    
    public MenuItem MenuItem { get; set; } = null!;
    public Language Language { get; set; } = null!;
}