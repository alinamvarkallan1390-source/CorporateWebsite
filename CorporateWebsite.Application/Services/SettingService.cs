using CorporateWebsite.Core.DTOs;
using CorporateWebsite.Core.Entities;
using CorporateWebsite.Core.Interfaces;
using CorporateWebsite.Application.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace CorporateWebsite.Application.Services;

public class SettingService : ISettingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILanguageService _languageService;

    public SettingService(IUnitOfWork unitOfWork, IMapper mapper, ILanguageService languageService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _languageService = languageService;
    }

    public async Task<SettingDto?> GetByKeyAsync(string key, string? languageCode = null)
    {
        var setting = await _unitOfWork.Settings.FirstOrDefaultAsync(s => s.Key == key, s => s.Translations);
        if (setting == null) return null;

        var dto = _mapper.Map<SettingDto>(setting);
        await SetTranslationAsync(dto, languageCode);
        
        return dto;
    }

    public async Task<T?> GetValueAsync<T>(string key, T? defaultValue = default)
    {
        var setting = await _unitOfWork.Settings.FirstOrDefaultAsync(s => s.Key == key);
        if (setting == null || string.IsNullOrEmpty(setting.Value))
        {
            return defaultValue;
        }

        try
        {
            if (typeof(T) == typeof(bool))
            {
                return (T)(object)bool.Parse(setting.Value);
            }
            else if (typeof(T) == typeof(int))
            {
                return (T)(object)int.Parse(setting.Value);
            }
            else if (typeof(T) == typeof(long))
            {
                return (T)(object)long.Parse(setting.Value);
            }
            else if (typeof(T) == typeof(double))
            {
                return (T)(object)double.Parse(setting.Value);
            }
            else if (typeof(T) == typeof(DateTime))
            {
                return (T)(object)DateTime.Parse(setting.Value);
            }
            else
            {
                return (T)(object)setting.Value;
            }
        }
        catch
        {
            return defaultValue;
        }
    }

    public async Task<string?> GetValueAsync(string key, string? defaultValue = null)
    {
        var setting = await _unitOfWork.Settings.FirstOrDefaultAsync(s => s.Key == key);
        return setting?.Value ?? defaultValue;
    }

    public async Task<SettingDto> SetValueAsync(string key, string value, string group = "General", string dataType = "String", bool isPublic = false, bool isEncrypted = false)
    {
        var setting = await _unitOfWork.Settings.FirstOrDefaultAsync(s => s.Key == key);
        
        if (setting == null)
        {
            setting = new Setting
            {
                Key = key,
                Value = value,
                Group = group,
                DataType = dataType,
                IsPublic = isPublic,
                IsEncrypted = isEncrypted,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Settings.AddAsync(setting);
        }
        else
        {
            setting.Value = value;
            setting.Group = group;
            setting.DataType = dataType;
            setting.IsPublic = isPublic;
            setting.IsEncrypted = isEncrypted;
            setting.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Settings.UpdateAsync(setting);
        }

        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<SettingDto>(setting);
    }

    public async Task<IReadOnlyList<SettingDto>> GetAllAsync(string? group = null, string? languageCode = null)
    {
        Expression<Func<Setting, bool>> predicate = s => true;
        
        if (!string.IsNullOrEmpty(group))
        {
            predicate = CombinePredicates(predicate, s => s.Group == group);
        }

        var settings = await _unitOfWork.Settings.GetAllAsync(predicate, q => q.OrderBy(s => s.Group).ThenBy(s => s.DisplayOrder), s => s.Translations);
        var dtos = _mapper.Map<IReadOnlyList<SettingDto>>(settings);
        await SetTranslationsAsync(dtos, languageCode);
        
        return dtos;
    }

    public async Task<IReadOnlyList<SettingDto>> GetPublicSettingsAsync(string languageCode)
    {
        var settings = await _unitOfWork.Settings.GetAllAsync(s => s.IsPublic, q => q.OrderBy(s => s.Group).ThenBy(s => s.DisplayOrder), s => s.Translations);
        var dtos = _mapper.Map<IReadOnlyList<SettingDto>>(settings);
        await SetTranslationsAsync(dtos, languageCode);
        return dtos;
    }

    public async Task<Dictionary<string, string>> GetSettingsDictionaryAsync(string? group = null, string? languageCode = null)
    {
        var settings = await GetAllAsync(group, languageCode);
        var dict = new Dictionary<string, string>();
        
        foreach (var setting in settings)
        {
            var value = setting.Translation?.Value ?? setting.Value;
            dict[setting.Key] = value ?? string.Empty;
        }
        
        return dict;
    }

    public async Task<bool> DeleteAsync(string key)
    {
        var setting = await _unitOfWork.Settings.FirstOrDefaultAsync(s => s.Key == key);
        if (setting == null) return false;

        await _unitOfWork.Settings.DeleteAsync(setting);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<SettingDto> UpdateAsync(int id, UpdateSettingDto dto)
    {
        var setting = await _unitOfWork.Settings.GetByIdAsync(id, s => s.Translations);
        if (setting == null)
        {
            throw new KeyNotFoundException($"Setting with id {id} not found.");
        }

        _mapper.Map(dto, setting);
        setting.UpdatedAt = DateTime.UtcNow;

        await UpdateTranslationsAsync(setting.Translations, dto.Translations, (t, d) => _mapper.Map(d, t));

        await _unitOfWork.Settings.UpdateAsync(setting);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<SettingDto>(setting);
    }

    public async Task InitializeDefaultSettingsAsync()
    {
        // This is handled in DbContext seed data
        await Task.CompletedTask;
    }

    private async Task SetTranslationAsync(SettingDto dto, string? languageCode)
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

    private async Task SetTranslationsAsync(IReadOnlyList<SettingDto> dtos, string? languageCode)
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

    private Expression<Func<Setting, bool>> CombinePredicates(Expression<Func<Setting, bool>> first, Expression<Func<Setting, bool>> second)
    {
        var parameter = Expression.Parameter(typeof(Setting));
        var firstBody = Expression.Invoke(first, parameter);
        var secondBody = Expression.Invoke(second, parameter);
        var combined = Expression.AndAlso(firstBody, secondBody);
        return Expression.Lambda<Func<Setting, bool>>(combined, parameter);
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