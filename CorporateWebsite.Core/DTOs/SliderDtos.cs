namespace CorporateWebsite.Core.DTOs;

public class SliderDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Location { get; set; } = "Homepage";
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? TransitionEffect { get; set; }
    public int? AutoPlayInterval { get; set; }
    public bool ShowNavigation { get; set; }
    public bool ShowPagination { get; set; }
    public bool PauseOnHover { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Translations
    public SliderTranslationDto? Translation { get; set; }
    public ICollection<SliderTranslationDto> Translations { get; set; } = new List<SliderTranslationDto>();
    
    // Items
    public ICollection<SliderItemDto> Items { get; set; } = new List<SliderItemDto>();
}

public class SliderTranslationDto
{
    public int Id { get; set; }
    public int SliderId { get; set; }
    public int LanguageId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CreateSliderDto
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
    public List<CreateSliderTranslationDto> Translations { get; set; } = new List<CreateSliderTranslationDto>();
    public List<CreateSliderItemDto> Items { get; set; } = new List<CreateSliderItemDto>();
}

public class CreateSliderTranslationDto
{
    public int LanguageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateSliderDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Location { get; set; } = "Homepage";
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? TransitionEffect { get; set; }
    public int? AutoPlayInterval { get; set; }
    public bool ShowNavigation { get; set; }
    public bool ShowPagination { get; set; }
    public bool PauseOnHover { get; set; }
    public List<UpdateSliderTranslationDto> Translations { get; set; } = new List<UpdateSliderTranslationDto>();
    public List<UpdateSliderItemDto> Items { get; set; } = new List<UpdateSliderItemDto>();
}

public class UpdateSliderTranslationDto
{
    public int? Id { get; set; }
    public int LanguageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class SliderItemDto
{
    public int Id { get; set; }
    public int SliderId { get; set; }
    public string? ImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public string? ButtonText { get; set; }
    public string? ButtonUrl { get; set; }
    public string? ButtonTarget { get; set; } = "_self";
    public string? ButtonStyle { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public string? ContentAlignment { get; set; }
    public string? ContentPosition { get; set; }
    public string? OverlayColor { get; set; }
    public double? OverlayOpacity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Translations
    public SliderItemTranslationDto? Translation { get; set; }
    public ICollection<SliderItemTranslationDto> Translations { get; set; } = new List<SliderItemTranslationDto>();
}

public class SliderItemTranslationDto
{
    public int Id { get; set; }
    public int SliderItemId { get; set; }
    public int LanguageId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Subtitle { get; set; }
    public string? Description { get; set; }
    public string? ButtonText { get; set; }
    public string? AltText { get; set; }
}

public class CreateSliderItemDto
{
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
    public List<CreateSliderItemTranslationDto> Translations { get; set; } = new List<CreateSliderItemTranslationDto>();
}

public class CreateSliderItemTranslationDto
{
    public int LanguageId { get; set; }
    public string? Title { get; set; }
    public string? Subtitle { get; set; }
    public string? Description { get; set; }
    public string? ButtonText { get; set; }
    public string? AltText { get; set; }
}

public class UpdateSliderItemDto
{
    public int? Id { get; set; }
    public string? ImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public string? ButtonText { get; set; }
    public string? ButtonUrl { get; set; }
    public string? ButtonTarget { get; set; }
    public string? ButtonStyle { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public string? ContentAlignment { get; set; }
    public string? ContentPosition { get; set; }
    public string? OverlayColor { get; set; }
    public double? OverlayOpacity { get; set; }
    public List<UpdateSliderItemTranslationDto> Translations { get; set; } = new List<UpdateSliderItemTranslationDto>();
}

public class UpdateSliderItemTranslationDto
{
    public int? Id { get; set; }
    public int LanguageId { get; set; }
    public string? Title { get; set; }
    public string? Subtitle { get; set; }
    public string? Description { get; set; }
    public string? ButtonText { get; set; }
    public string? AltText { get; set; }
}

public class BannerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Location { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public string? LinkUrl { get; set; }
    public string? LinkTarget { get; set; } = "_self";
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? CssClass { get; set; }
    public string? CustomHtml { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Translations
    public BannerTranslationDto? Translation { get; set; }
    public ICollection<BannerTranslationDto> Translations { get; set; } = new List<BannerTranslationDto>();
}

public class BannerTranslationDto
{
    public int Id { get; set; }
    public int BannerId { get; set; }
    public int LanguageId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? AltText { get; set; }
    public string? ButtonText { get; set; }
    public string? CustomHtml { get; set; }
}

public class CreateBannerDto
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
    public List<CreateBannerTranslationDto> Translations { get; set; } = new List<CreateBannerTranslationDto>();
}

public class CreateBannerTranslationDto
{
    public int LanguageId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? AltText { get; set; }
    public string? ButtonText { get; set; }
    public string? CustomHtml { get; set; }
}

public class UpdateBannerDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Location { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public string? LinkUrl { get; set; }
    public string? LinkTarget { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? CssClass { get; set; }
    public string? CustomHtml { get; set; }
    public List<UpdateBannerTranslationDto> Translations { get; set; } = new List<UpdateBannerTranslationDto>();
}

public class UpdateBannerTranslationDto
{
    public int? Id { get; set; }
    public int LanguageId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? AltText { get; set; }
    public string? ButtonText { get; set; }
    public string? CustomHtml { get; set; }
}