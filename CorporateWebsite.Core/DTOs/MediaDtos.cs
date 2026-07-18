namespace CorporateWebsite.Core.DTOs;

public class MediaFileDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string FileExtension { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public string? AltText { get; set; }
    public string? Caption { get; set; }
    public string? Description { get; set; }
    public string Folder { get; set; } = "uploads";
    public string? Tags { get; set; }
    public int DownloadCount { get; set; }
    public bool IsPublic { get; set; }
    public string? UploadedBy { get; set; }
    public string? EntityType { get; set; }
    public int? EntityId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Translations
    public MediaFileTranslationDto? Translation { get; set; }
    public ICollection<MediaFileTranslationDto> Translations { get; set; } = new List<MediaFileTranslationDto>();
}

public class MediaFileTranslationDto
{
    public int Id { get; set; }
    public int MediaFileId { get; set; }
    public int LanguageId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public string? Caption { get; set; }
    public string? Description { get; set; }
}

public class UploadMediaDto
{
    public string? Folder { get; set; }
    public string? AltText { get; set; }
    public string? Caption { get; set; }
    public string? Description { get; set; }
    public string? Tags { get; set; }
    public bool IsPublic { get; set; } = true;
    public string? EntityType { get; set; }
    public int? EntityId { get; set; }
    public List<CreateMediaFileTranslationDto> Translations { get; set; } = new List<CreateMediaFileTranslationDto>();
}

public class CreateMediaFileTranslationDto
{
    public int LanguageId { get; set; }
    public string? AltText { get; set; }
    public string? Caption { get; set; }
    public string? Description { get; set; }
}

public class UpdateMediaDto
{
    public string? AltText { get; set; }
    public string? Caption { get; set; }
    public string? Description { get; set; }
    public string? Folder { get; set; }
    public string? Tags { get; set; }
    public bool IsPublic { get; set; }
    public List<UpdateMediaFileTranslationDto> Translations { get; set; } = new List<UpdateMediaFileTranslationDto>();
}

public class UpdateMediaFileTranslationDto
{
    public int? Id { get; set; }
    public int LanguageId { get; set; }
    public string? AltText { get; set; }
    public string? Caption { get; set; }
    public string? Description { get; set; }
}

public class MediaFolderDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public int DisplayOrder { get; set; }
    public string? Icon { get; set; }
    public bool IsSystem { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Translations
    public MediaFolderTranslationDto? Translation { get; set; }
    public ICollection<MediaFolderTranslationDto> Translations { get; set; } = new List<MediaFolderTranslationDto>();
    
    // Children
    public ICollection<MediaFolderDto> Children { get; set; } = new List<MediaFolderDto>();
}

public class MediaFolderTranslationDto
{
    public int Id { get; set; }
    public int FolderId { get; set; }
    public int LanguageId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class CreateMediaFolderDto
{
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public int DisplayOrder { get; set; }
    public string? Icon { get; set; }
    public List<CreateMediaFolderTranslationDto> Translations { get; set; } = new List<CreateMediaFolderTranslationDto>();
}

public class CreateMediaFolderTranslationDto
{
    public int LanguageId { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class UpdateMediaFolderDto
{
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public int DisplayOrder { get; set; }
    public string? Icon { get; set; }
    public List<UpdateMediaFolderTranslationDto> Translations { get; set; } = new List<UpdateMediaFolderTranslationDto>();
}

public class UpdateMediaFolderTranslationDto
{
    public int? Id { get; set; }
    public int LanguageId { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class MediaFilterDto
{
    public string? Folder { get; set; }
    public string? Search { get; set; }
    public string? FileType { get; set; } // image, video, document, other
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "CreatedAt";
    public string? SortDirection { get; set; } = "desc";
}