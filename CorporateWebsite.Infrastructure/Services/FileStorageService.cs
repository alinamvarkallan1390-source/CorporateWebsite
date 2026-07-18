using CorporateWebsite.Core.DTOs;
using CorporateWebsite.Core.Entities;
using CorporateWebsite.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CorporateWebsite.Infrastructure.Services;

public interface IFileStorageService
{
    Task<MediaFileDto> UploadAsync(IFormFile file, UploadMediaDto dto, string uploadedBy);
    Task<MediaFileDto> UploadAsync(Stream fileStream, string fileName, string contentType, UploadMediaDto dto, string uploadedBy);
    Task<byte[]> GetFileAsync(int id);
    Task<Stream> GetFileStreamAsync(int id);
    Task<bool> DeleteAsync(int id);
    Task<bool> DeleteRangeAsync(IEnumerable<int> ids);
    Task<string> GetFileUrlAsync(int id);
    Task<MediaFileDto> GetByIdAsync(int id, string? languageCode = null);
    Task<IReadOnlyList<MediaFileDto>> GetPagedAsync(MediaFilterDto filter);
    Task<MediaFolderDto?> GetFolderByIdAsync(int id, string? languageCode = null);
    Task<IReadOnlyList<MediaFolderDto>> GetFoldersAsync(int? parentId = null, string? languageCode = null);
    Task<MediaFolderDto> CreateFolderAsync(CreateMediaFolderDto dto);
    Task<MediaFolderDto> UpdateFolderAsync(int id, UpdateMediaFolderDto dto);
    Task DeleteFolderAsync(int id);
}

public class FileStorageService : IFileStorageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILanguageService _languageService;
    private readonly IConfiguration _configuration;
    private readonly IImageOptimizationService _imageOptimizationService;
    private readonly string _uploadPath;
    private readonly long _maxFileSize;
    private readonly string[] _allowedImageTypes;
    private readonly string[] _allowedDocumentTypes;
    private readonly string[] _allowedVideoTypes;

    public FileStorageService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILanguageService languageService,
        IConfiguration configuration,
        IImageOptimizationService imageOptimizationService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _languageService = languageService;
        _configuration = configuration;
        _imageOptimizationService = imageOptimizationService;
        
        _uploadPath = configuration["Upload:Path"] ?? "uploads";
        _maxFileSize = configuration.GetValue<long>("Upload:MaxFileSizeMb", 10) * 1024 * 1024;
        _allowedImageTypes = (configuration["Upload:AllowedImageTypes"] ?? "image/jpeg,image/png,image/gif,image/webp,image/avif").Split(',');
        _allowedDocumentTypes = (configuration["Upload:AllowedDocumentTypes"] ?? "application/pdf,application/msword,application/vnd.openxmlformats-officedocument.wordprocessingml.document").Split(',');
        _allowedVideoTypes = (configuration["Upload:AllowedVideoTypes"] ?? "video/mp4,video/webm,video/ogg").Split(',');

        // Ensure upload directory exists
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", _uploadPath);
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }
    }

    public async Task<MediaFileDto> UploadAsync(IFormFile file, UploadMediaDto dto, string uploadedBy)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("No file provided.");
        }

        if (file.Length > _maxFileSize)
        {
            throw new ArgumentException($"File size exceeds maximum allowed size of {_maxFileSize / (1024 * 1024)} MB.");
        }

        if (!IsAllowedFileType(file.ContentType))
        {
            throw new ArgumentException($"File type '{file.ContentType}' is not allowed.");
        }

        using var stream = file.OpenReadStream();
        return await UploadAsync(stream, file.FileName, file.ContentType, dto, uploadedBy);
    }

    public async Task<MediaFileDto> UploadAsync(Stream fileStream, string fileName, string contentType, UploadMediaDto dto, string uploadedBy)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var storedFileName = $"{Guid.NewGuid()}{extension}";
        var folder = string.IsNullOrEmpty(dto.Folder) ? _uploadPath : Path.Combine(_uploadPath, dto.Folder);
        var dateFolder = DateTime.UtcNow.ToString("yyyy/MM/dd");
        var fullFolder = Path.Combine(folder, dateFolder);
        
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fullFolder);
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }

        var filePath = Path.Combine(fullPath, storedFileName);
        var relativePath = Path.Combine(fullFolder, storedFileName).Replace("\\", "/");

        // Save original file
        fileStream.Position = 0;
        using (var fileOutput = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(fileOutput);
        }

        // Get dimensions for images
        int? width = null, height = null;
        if (IsImageFile(contentType))
        {
            fileStream.Position = 0;
            (width, height) = await _imageOptimizationService.GetDimensionsAsync(fileStream);
        }

        // Create media file entity
        var mediaFile = new MediaFile
        {
            FileName = storedFileName,
            OriginalFileName = fileName,
            FilePath = relativePath,
            ContentType = contentType,
            FileExtension = extension,
            FileSize = new FileInfo(filePath).Length,
            Width = width,
            Height = height,
            Folder = fullFolder.Replace("\\", "/"),
            Tags = dto.Tags,
            IsPublic = dto.IsPublic,
            UploadedBy = uploadedBy,
            EntityType = dto.EntityType,
            EntityId = dto.EntityId,
            CreatedAt = DateTime.UtcNow
        };

        // Add translations
        foreach (var transDto in dto.Translations)
        {
            var translation = _mapper.Map<MediaFileTranslation>(transDto);
            translation.MediaFile = mediaFile;
            mediaFile.Translations.Add(translation);
        }

        await _unitOfWork.MediaFiles.AddAsync(mediaFile);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<MediaFileDto>(mediaFile);
    }

    public async Task<byte[]> GetFileAsync(int id)
    {
        var mediaFile = await _unitOfWork.MediaFiles.GetByIdAsync(id);
        if (mediaFile == null)
        {
            throw new KeyNotFoundException($"Media file with id {id} not found.");
        }

        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", mediaFile.FilePath);
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("File not found on disk.", fullPath);
        }

        return await File.ReadAllBytesAsync(fullPath);
    }

    public async Task<Stream> GetFileStreamAsync(int id)
    {
        var mediaFile = await _unitOfWork.MediaFiles.GetByIdAsync(id);
        if (mediaFile == null)
        {
            throw new KeyNotFoundException($"Media file with id {id} not found.");
        }

        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", mediaFile.FilePath);
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("File not found on disk.", fullPath);
        }

        return new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var mediaFile = await _unitOfWork.MediaFiles.GetByIdAsync(id);
        if (mediaFile == null) return false;

        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", mediaFile.FilePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        await _unitOfWork.MediaFiles.DeleteAsync(mediaFile);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteRangeAsync(IEnumerable<int> ids)
    {
        var mediaFiles = await _unitOfWork.MediaFiles.GetAllAsync(m => ids.Contains(m.Id));
        foreach (var mediaFile in mediaFiles)
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", mediaFile.FilePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        await _unitOfWork.MediaFiles.DeleteRangeAsync(mediaFiles);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<string> GetFileUrlAsync(int id)
    {
        var mediaFile = await _unitOfWork.MediaFiles.GetByIdAsync(id);
        if (mediaFile == null) return string.Empty;

        return $"/{mediaFile.FilePath}";
    }

    public async Task<MediaFileDto> GetByIdAsync(int id, string? languageCode = null)
    {
        var mediaFile = await _unitOfWork.MediaFiles.GetByIdAsync(id, m => m.Translations);
        if (mediaFile == null) return null;

        var dto = _mapper.Map<MediaFileDto>(mediaFile);
        await SetTranslationAsync(dto, languageCode);
        return dto;
    }

    public async Task<IReadOnlyList<MediaFileDto>> GetPagedAsync(MediaFilterDto filter)
    {
        Expression<Func<MediaFile, bool>> predicate = m => true;

        if (!string.IsNullOrEmpty(filter.Folder))
        {
            predicate = CombinePredicates(predicate, m => m.Folder.Contains(filter.Folder!));
        }

        if (!string.IsNullOrEmpty(filter.Search))
        {
            var search = filter.Search.ToLower();
            predicate = CombinePredicates(predicate, m => 
                m.OriginalFileName.ToLower().Contains(search) ||
                m.FileName.ToLower().Contains(search) ||
                m.Tags != null && m.Tags.ToLower().Contains(search));
        }

        if (!string.IsNullOrEmpty(filter.FileType))
        {
            predicate = CombinePredicates(predicate, m => GetFileCategory(m.ContentType) == filter.FileType);
        }

        if (filter.FromDate.HasValue)
        {
            predicate = CombinePredicates(predicate, m => m.CreatedAt >= filter.FromDate.Value);
        }

        if (filter.ToDate.HasValue)
        {
            predicate = CombinePredicates(predicate, m => m.CreatedAt <= filter.ToDate.Value);
        }

        var (items, _) = await _unitOfWork.MediaFiles.GetPagedAsync(
            filter.Page, 
            filter.PageSize, 
            predicate, 
            BuildOrderBy(filter.SortBy, filter.SortDirection),
            m => m.Translations);

        var dtos = _mapper.Map<IReadOnlyList<MediaFileDto>>(items);
        await SetTranslationsAsync(dtos, filter.LanguageCode);
        
        return dtos;
    }

    public async Task<MediaFolderDto?> GetFolderByIdAsync(int id, string? languageCode = null)
    {
        var folder = await _unitOfWork.MediaFolders.GetByIdAsync(id, f => f.Translations, f => f.Children);
        if (folder == null) return null;

        var dto = _mapper.Map<MediaFolderDto>(folder);
        await SetFolderTranslationAsync(dto, languageCode);
        return dto;
    }

    public async Task<IReadOnlyList<MediaFolderDto>> GetFoldersAsync(int? parentId = null, string? languageCode = null)
    {
        var folders = await _unitOfWork.MediaFolders.GetAllAsync(
            f => f.ParentId == parentId, 
            q => q.OrderBy(f => f.DisplayOrder), 
            f => f.Translations, 
            f => f.Children);

        var dtos = _mapper.Map<IReadOnlyList<MediaFolderDto>>(folders);
        await SetFolderTranslationsAsync(dtos, languageCode);
        return dtos;
    }

    public async Task<MediaFolderDto> CreateFolderAsync(CreateMediaFolderDto dto)
    {
        var folder = _mapper.Map<MediaFolder>(dto);
        folder.Path = dto.ParentId.HasValue 
            ? (await _unitOfWork.MediaFolders.GetByIdAsync(dto.ParentId.Value))?.Path + "/" + dto.Name 
            : dto.Name;
        folder.CreatedAt = DateTime.UtcNow;

        foreach (var transDto in dto.Translations)
        {
            var translation = _mapper.Map<MediaFolderTranslation>(transDto);
            translation.Folder = folder;
            folder.Translations.Add(translation);
        }

        await _unitOfWork.MediaFolders.AddAsync(folder);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<MediaFolderDto>(folder);
    }

    public async Task<MediaFolderDto> UpdateFolderAsync(int id, UpdateMediaFolderDto dto)
    {
        var folder = await _unitOfWork.MediaFolders.GetByIdAsync(id, f => f.Translations);
        if (folder == null)
        {
            throw new KeyNotFoundException($"Media folder with id {id} not found.");
        }

        _mapper.Map(dto, folder);
        folder.UpdatedAt = DateTime.UtcNow;

        await UpdateTranslationsAsync(folder.Translations, dto.Translations, (t, d) => _mapper.Map(d, t));

        await _unitOfWork.MediaFolders.UpdateAsync(folder);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<MediaFolderDto>(folder);
    }

    public async Task DeleteFolderAsync(int id)
    {
        var folder = await _unitOfWork.MediaFolders.GetByIdAsync(id, f => f.Children, f => f.MediaFiles);
        if (folder == null)
        {
            throw new KeyNotFoundException($"Media folder with id {id} not found.");
        }

        // Check for children
        if (folder.Children.Any())
        {
            throw new InvalidOperationException("Cannot delete folder that has subfolders. Delete subfolders first.");
        }

        // Check for files
        if (folder.MediaFiles.Any())
        {
            throw new InvalidOperationException("Cannot delete folder that contains files. Move or delete files first.");
        }

        await _unitOfWork.MediaFolders.DeleteAsync(folder);
        await _unitOfWork.SaveChangesAsync();
    }

    private bool IsAllowedFileType(string contentType)
    {
        return _allowedImageTypes.Contains(contentType) || 
               _allowedDocumentTypes.Contains(contentType) || 
               _allowedVideoTypes.Contains(contentType);
    }

    private bool IsImageFile(string contentType)
    {
        return _allowedImageTypes.Contains(contentType);
    }

    private string GetFileCategory(string contentType)
    {
        if (_allowedImageTypes.Contains(contentType)) return "image";
        if (_allowedVideoTypes.Contains(contentType)) return "video";
        if (_allowedDocumentTypes.Contains(contentType)) return "document";
        return "other";
    }

    private async Task SetTranslationAsync(MediaFileDto dto, string? languageCode)
    {
        if (!string.IsNullOrEmpty(languageCode))
        {
            var language = await _languageService.GetByCodeAsync(languageCode);
            if (language != null)
            {
                dto.Translation = dto.Translations.FirstOrDefault(t => t.LanguageId == language.Id);
            }
        }
    }

    private async Task SetTranslationsAsync(IReadOnlyList<MediaFileDto> dtos, string? languageCode)
    {
        if (!string.IsNullOrEmpty(languageCode))
        {
            var language = await _languageService.GetByCodeAsync(languageCode);
            if (language != null)
            {
                foreach (var dto in dtos)
                {
                    dto.Translation = dto.Translations.FirstOrDefault(t => t.LanguageId == language.Id);
                }
            }
        }
    }

    private async Task SetFolderTranslationAsync(MediaFolderDto dto, string? languageCode)
    {
        if (!string.IsNullOrEmpty(languageCode))
        {
            var language = await _languageService.GetByCodeAsync(languageCode);
            if (language != null)
            {
                dto.Translation = dto.Translations.FirstOrDefault(t => t.LanguageId == language.Id);
                
                foreach (var child in dto.Children)
                {
                    await SetFolderTranslationAsync(child, languageCode);
                }
            }
        }
    }

    private async Task SetFolderTranslationsAsync(IReadOnlyList<MediaFolderDto> dtos, string? languageCode)
    {
        if (!string.IsNullOrEmpty(languageCode))
        {
            var language = await _languageService.GetByCodeAsync(languageCode);
            if (language != null)
            {
                foreach (var dto in dtos)
                {
                    await SetFolderTranslationAsync(dto, languageCode);
                }
            }
        }
    }

    private Expression<Func<MediaFile, bool>> CombinePredicates(Expression<Func<MediaFile, bool>> first, Expression<Func<MediaFile, bool>> second)
    {
        var parameter = Expression.Parameter(typeof(MediaFile));
        var firstBody = Expression.Invoke(first, parameter);
        var secondBody = Expression.Invoke(second, parameter);
        var combined = Expression.AndAlso(firstBody, secondBody);
        return Expression.Lambda<Func<MediaFile, bool>>(combined, parameter);
    }

    private Func<IQueryable<MediaFile>, IOrderedQueryable<MediaFile>> BuildOrderBy(string? sortBy, string? sortDirection)
    {
        var isDesc = sortDirection?.ToLower() == "desc";
        
        return sortBy?.ToLower() switch
        {
            "filename" => isDesc ? q => q.OrderByDescending(m => m.OriginalFileName) : q => q.OrderBy(m => m.OriginalFileName),
            "filesize" => isDesc ? q => q.OrderByDescending(m => m.FileSize) : q => q.OrderBy(m => m.FileSize),
            "createdat" => isDesc ? q => q.OrderByDescending(m => m.CreatedAt) : q => q.OrderBy(m => m.CreatedAt),
            _ => isDesc ? q => q.OrderByDescending(m => m.CreatedAt) : q => q.OrderByDescending(m => m.CreatedAt)
        };
    }

    private async Task UpdateTranslationsAsync<TEntity, TDto>(
        ICollection<TEntity> existing, 
        List<TDto> dtos,
        Action<TEntity, TDto> updateAction)
        where TEntity : BaseEntity
    {
        var existingDict = existing.ToDictionary(e => e.Id);
        var processedIds = new HashSet<int>();

        foreach (var dto in dtos)
        {
            var idProp = typeof(TDto).GetProperty("Id");
            var id = idProp?.GetValue(dto) as int?;
            
            if (id.HasValue && existingDict.TryGetValue(id.Value, out var entity))
            {
                updateAction(entity, dto);
                entity.UpdatedAt = DateTime.UtcNow;
                processedIds.Add(id.Value);
            }
        }

        foreach (var entity in existing.Where(e => !processedIds.Contains(e.Id)))
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
        }
    }
}