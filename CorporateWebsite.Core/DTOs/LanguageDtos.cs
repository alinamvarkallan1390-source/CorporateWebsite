namespace CorporateWebsite.Core.DTOs;

public class LanguageDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NativeName { get; set; } = string.Empty;
    public string CultureCode { get; set; } = string.Empty;
    public bool IsRtl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public bool IsDefault { get; set; }
    public string? FlagIcon { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateLanguageDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NativeName { get; set; } = string.Empty;
    public string CultureCode { get; set; } = string.Empty;
    public bool IsRtl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDefault { get; set; }
    public string? FlagIcon { get; set; }
}

public class UpdateLanguageDto
{
    public string Name { get; set; } = string.Empty;
    public string NativeName { get; set; } = string.Empty;
    public string CultureCode { get; set; } = string.Empty;
    public bool IsRtl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public bool IsDefault { get; set; }
    public string? FlagIcon { get; set; }
}

public class ReorderItemDto
{
    public int Id { get; set; }
    public int DisplayOrder { get; set; }
    public int? ParentId { get; set; }
}