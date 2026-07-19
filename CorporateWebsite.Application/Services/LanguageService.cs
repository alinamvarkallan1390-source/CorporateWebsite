using CorporateWebsite.Core.DTOs;
using System.Linq.Expressions;
using CorporateWebsite.Core.Entities;
using CorporateWebsite.Core.Interfaces;
using CorporateWebsite.Application.Interfaces;
using AutoMapper;

namespace CorporateWebsite.Application.Services;

public class LanguageService : ILanguageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public LanguageService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<LanguageDto?> GetByIdAsync(int id)
    {
        var language = await _unitOfWork.Languages.GetByIdAsync(id);
        return language != null ? _mapper.Map<LanguageDto>(language) : null;
    }

    public async Task<LanguageDto?> GetByCodeAsync(string code)
    {
        var language = await _unitOfWork.Languages.FirstOrDefaultAsync(l => l.Code == code);
        return language != null ? _mapper.Map<LanguageDto>(language) : null;
    }

    public async Task<LanguageDto?> GetDefaultAsync()
    {
        var language = await _unitOfWork.Languages.FirstOrDefaultAsync(l => l.IsDefault && l.IsActive);
        return language != null ? _mapper.Map<LanguageDto>(language) : null;
    }

    public async Task<IReadOnlyList<LanguageDto>> GetAllActiveAsync()
    {
        var languages = await _unitOfWork.Languages.GetAllAsync(l => l.IsActive, q => q.OrderBy(l => l.DisplayOrder));
        return _mapper.Map<IReadOnlyList<LanguageDto>>(languages);
    }

    public async Task<IReadOnlyList<LanguageDto>> GetAllAsync()
    {
        var languages = await _unitOfWork.Languages.GetAllAsync(q => q.OrderBy(l => l.DisplayOrder));
        return _mapper.Map<IReadOnlyList<LanguageDto>>(languages);
    }

    public async Task<LanguageDto> CreateAsync(CreateLanguageDto dto)
    {
        if (await ExistsCodeAsync(dto.Code))
        {
            throw new InvalidOperationException($"Language with code '{dto.Code}' already exists.");
        }

        // If this is set as default, unset others
        if (dto.IsDefault)
        {
            await UnsetDefaultAsync();
        }

        var language = _mapper.Map<Language>(dto);
        language.CreatedAt = DateTime.UtcNow;
        
        await _unitOfWork.Languages.AddAsync(language);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<LanguageDto>(language);
    }

    public async Task<LanguageDto> UpdateAsync(int id, UpdateLanguageDto dto)
    {
        var language = await _unitOfWork.Languages.GetByIdAsync(id);
        if (language == null)
        {
            throw new KeyNotFoundException($"Language with id {id} not found.");
        }

        // Check if code is being changed and if new code exists
        if (language.Code != dto.Code && await ExistsCodeAsync(dto.Code, id))
        {
            throw new InvalidOperationException($"Language with code '{dto.Code}' already exists.");
        }

        // If this is set as default, unset others
        if (dto.IsDefault && !language.IsDefault)
        {
            await UnsetDefaultAsync();
        }

        _mapper.Map(dto, language);
        language.UpdatedAt = DateTime.UtcNow;
        
        await _unitOfWork.Languages.UpdateAsync(language);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<LanguageDto>(language);
    }

    public async Task DeleteAsync(int id)
    {
        var language = await _unitOfWork.Languages.GetByIdAsync(id);
        if (language == null)
        {
            throw new KeyNotFoundException($"Language with id {id} not found.");
        }

        if (language.IsDefault)
        {
            throw new InvalidOperationException("Cannot delete the default language.");
        }

        // Check if language has translations
        var hasTranslations = await _unitOfWork.PageTranslations.ExistsAsync(t => t.LanguageId == id) ||
                             await _unitOfWork.ServiceTranslations.ExistsAsync(t => t.LanguageId == id) ||
                             await _unitOfWork.ProjectTranslations.ExistsAsync(t => t.LanguageId == id) ||
                             await _unitOfWork.NewsTranslations.ExistsAsync(t => t.LanguageId == id);

        if (hasTranslations)
        {
            throw new InvalidOperationException("Cannot delete language that has translations. Remove translations first.");
        }

        await _unitOfWork.Languages.DeleteAsync(language);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task SetDefaultAsync(int id)
    {
        await UnsetDefaultAsync();
        
        var language = await _unitOfWork.Languages.GetByIdAsync(id);
        if (language == null)
        {
            throw new KeyNotFoundException($"Language with id {id} not found.");
        }

        language.IsDefault = true;
        language.UpdatedAt = DateTime.UtcNow;
        
        await _unitOfWork.Languages.UpdateAsync(language);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ReorderAsync(List<ReorderItemDto> items)
    {
        foreach (var item in items)
        {
            var language = await _unitOfWork.Languages.GetByIdAsync(item.Id);
            if (language != null)
            {
                language.DisplayOrder = item.DisplayOrder;
                language.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Languages.UpdateAsync(language);
            }
        }
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> ExistsCodeAsync(string code, int? excludeId = null)
    {
        return await _unitOfWork.Languages.ExistsAsync(l => l.Code == code && (!excludeId.HasValue || l.Id != excludeId.Value));
    }

    public async Task<LanguageDto?> GetByCultureCodeAsync(string cultureCode)
    {
        var language = await _unitOfWork.Languages.FirstOrDefaultAsync(l => l.CultureCode == cultureCode);
        return language != null ? _mapper.Map<LanguageDto>(language) : null;
    }

    private async Task UnsetDefaultAsync()
    {
        var currentDefault = await _unitOfWork.Languages.FirstOrDefaultAsync(l => l.IsDefault);
        if (currentDefault != null)
        {
            currentDefault.IsDefault = false;
            currentDefault.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Languages.UpdateAsync(currentDefault);
        }
    }
}