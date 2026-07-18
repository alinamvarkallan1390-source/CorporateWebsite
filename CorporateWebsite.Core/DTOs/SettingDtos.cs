namespace CorporateWebsite.Core.DTOs;

public class SettingDto
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string? Value { get; set; }
    public string Group { get; set; } = "General";
    public string DataType { get; set; } = "String";
    public string? Description { get; set; }
    public bool IsPublic { get; set; }
    public bool IsEncrypted { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Translations
    public SettingTranslationDto? Translation { get; set; }
    public ICollection<SettingTranslationDto> Translations { get; set; } = new List<SettingTranslationDto>();
}

public class SettingTranslationDto
{
    public int Id { get; set; }
    public int SettingId { get; set; }
    public int LanguageId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public string? Value { get; set; }
    public string? Description { get; set; }
}

public class UpdateSettingDto
{
    public string? Value { get; set; }
    public string? Description { get; set; }
    public bool IsPublic { get; set; }
    public bool IsEncrypted { get; set; }
    public int DisplayOrder { get; set; }
    public List<UpdateSettingTranslationDto> Translations { get; set; } = new List<UpdateSettingTranslationDto>();
}

public class UpdateSettingTranslationDto
{
    public int? Id { get; set; }
    public int LanguageId { get; set; }
    public string? Value { get; set; }
    public string? Description { get; set; }
}

public class SettingGroupDto
{
    public string GroupName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ICollection<SettingDto> Settings { get; set; } = new List<SettingDto>();
}