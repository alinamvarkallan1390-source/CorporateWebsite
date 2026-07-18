namespace CorporateWebsite.Core.DTOs;

public class MenuDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Translations
    public MenuTranslationDto? Translation { get; set; }
    public ICollection<MenuTranslationDto> Translations { get; set; } = new List<MenuTranslationDto>();
    
    // Items
    public ICollection<MenuItemDto> Items { get; set; } = new List<MenuItemDto>();
}

public class MenuTranslationDto
{
    public int Id { get; set; }
    public int MenuId { get; set; }
    public int LanguageId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CreateMenuDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public List<CreateMenuTranslationDto> Translations { get; set; } = new List<CreateMenuTranslationDto>();
}

public class CreateMenuTranslationDto
{
    public int LanguageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateMenuDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public List<UpdateMenuTranslationDto> Translations { get; set; } = new List<UpdateMenuTranslationDto>();
}

public class UpdateMenuTranslationDto
{
    public int? Id { get; set; }
    public int LanguageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class MenuItemDto
{
    public int Id { get; set; }
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
    public bool IsActive { get; set; }
    public bool IsMegaMenu { get; set; }
    public string? MegaMenuContent { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Translations
    public MenuItemTranslationDto? Translation { get; set; }
    public ICollection<MenuItemTranslationDto> Translations { get; set; } = new List<MenuItemTranslationDto>();
    
    // Children
    public ICollection<MenuItemDto> Children { get; set; } = new List<MenuItemDto>();
    
    // Linked entities
    public PageDto? Page { get; set; }
    public ServiceDto? Service { get; set; }
    public ProjectDto? Project { get; set; }
    public NewsCategoryDto? NewsCategory { get; set; }
}

public class MenuItemTranslationDto
{
    public int Id { get; set; }
    public int MenuItemId { get; set; }
    public int LanguageId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Tooltip { get; set; }
}

public class CreateMenuItemDto
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
    public List<CreateMenuItemTranslationDto> Translations { get; set; } = new List<CreateMenuItemTranslationDto>();
}

public class CreateMenuItemTranslationDto
{
    public int LanguageId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Tooltip { get; set; }
}

public class UpdateMenuItemDto
{
    public int? ParentId { get; set; }
    public int? PageId { get; set; }
    public int? ServiceId { get; set; }
    public int? ProjectId { get; set; }
    public int? NewsCategoryId { get; set; }
    public string? CustomUrl { get; set; }
    public string? Target { get; set; }
    public string? CssClass { get; set; }
    public string? Icon { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public bool IsMegaMenu { get; set; }
    public string? MegaMenuContent { get; set; }
    public List<UpdateMenuItemTranslationDto> Translations { get; set; } = new List<UpdateMenuItemTranslationDto>();
}

public class UpdateMenuItemTranslationDto
{
    public int? Id { get; set; }
    public int LanguageId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Tooltip { get; set; }
}