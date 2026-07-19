namespace CorporateWebsite.Core.Entities;

public class Slider : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Location { get; set; } = "Homepage";
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? TransitionEffect { get; set; }
    public int? AutoPlayInterval { get; set; }
    public bool ShowNavigation { get; set; } = true;
    public bool ShowPagination { get; set; } = true;
    public bool PauseOnHover { get; set; } = true;
    
    public ICollection<SliderTranslation> Translations { get; set; } = new List<SliderTranslation>();
    public ICollection<SliderItem> Items { get; set; } = new List<SliderItem>();
}

public class SliderTranslation : BaseEntity
{
    public int SliderId { get; set; }
    public int LanguageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    public Slider Slider { get; set; } = null!;
    public Language Language { get; set; } = null!;
}

public class SliderItem : BaseEntity
{
    public int SliderId { get; set; }
    public string? ImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public string? ButtonText { get; set; }
    public string? ButtonUrl { get; set; }
    public string? ButtonTarget { get; set; } = "_self";
    public string? ButtonStyle { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public string? ContentAlignment { get; set; }
    public string? ContentPosition { get; set; }
    public string? OverlayColor { get; set; }
    public double? OverlayOpacity { get; set; }
    
    public Slider Slider { get; set; } = null!;
    public ICollection<SliderItemTranslation> Translations { get; set; } = new List<SliderItemTranslation>();
}

public class SliderItemTranslation : BaseEntity
{
    public int SliderItemId { get; set; }
    public int LanguageId { get; set; }
    public string? Title { get; set; }
    public string? Subtitle { get; set; }
    public string? Description { get; set; }
    public string? ButtonText { get; set; }
    public string? AltText { get; set; }
    
    public SliderItem SliderItem { get; set; } = null!;
    public Language Language { get; set; } = null!;
}

public class Banner : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Location { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public string? LinkUrl { get; set; }
    public string? LinkTarget { get; set; } = "_self";
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? CssClass { get; set; }
    public string? CustomHtml { get; set; }
    
    public ICollection<BannerTranslation> Translations { get; set; } = new List<BannerTranslation>();
}

public class BannerTranslation : BaseEntity
{
    public int BannerId { get; set; }
    public int LanguageId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? AltText { get; set; }
    public string? ButtonText { get; set; }
    public string? CustomHtml { get; set; }
    
    public Banner Banner { get; set; } = null!;
    public Language Language { get; set; } = null!;
}