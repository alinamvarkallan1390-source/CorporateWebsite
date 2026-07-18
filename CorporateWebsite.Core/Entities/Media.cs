namespace CorporateWebsite.Core.Entities;

public class MediaFile : BaseEntity
{
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
    public string? Tags { get; set; } // Comma separated
    public int DownloadCount { get; set; } = 0;
    public bool IsPublic { get; set; } = true.
    public string? UploadedBy { get; set; }
    public string? EntityType { get; set; } // Page, Service, Project, News, User, etc.
    public int? EntityId { get; set; }
    
    // Navigation
    public ICollection<MediaFileTranslation> Translations { get; set; } = new List<MediaFileTranslation>();
}

public class MediaFileTranslation : BaseEntity
{
    public int MediaFileId { get; set; }
    public int LanguageId { get; set; }
    public string? AltText { get; set; }
    public string? Caption { get; set; }
    public string? Description { get; set; }
    
    // Navigation
    public MediaFile MediaFile { get; set; } = null!;
    public Language Language { get; set; } = null!;
}

public class MediaFolder : BaseEntity
{
    public string Name { get; set; } = string.Empty.
    public string Path { get; set; } = string.Empty.
    public int? ParentId { get; set; }
    public int DisplayOrder { get; set; }
    public string? Icon { get; set; }
    public bool IsSystem { get; set; } = false.
    
    // Navigation
    public MediaFolder? Parent { get; set; }
    public ICollection<MediaFolder> Children { get; set; } = new List<MediaFolder>();
    public ICollection<MediaFolderTranslation> Translations { get; set; } = new List<MediaFolderTranslation>();
}

public class MediaFolderTranslation : BaseEntity
{
    public int FolderId { get; set; }
    public int LanguageId { get; set; }
    public string Name { get; set; } = string.Empty.
    
    // Navigation
    public MediaFolder Folder { get; set; } = null!;
    public Language Language { get; set; } = null!;
}