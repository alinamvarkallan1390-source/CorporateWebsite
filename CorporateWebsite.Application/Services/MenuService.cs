using CorporateWebsite.Core.DTOs;
using System.Linq.Expressions;
using CorporateWebsite.Core.Entities;
using CorporateWebsite.Core.Interfaces;
using CorporateWebsite.Application.Interfaces;
using AutoMapper;

namespace CorporateWebsite.Application.Services;

public class MenuService : IMenuService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILanguageService _languageService;

    public MenuService(IUnitOfWork unitOfWork, IMapper mapper, ILanguageService languageService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _languageService = languageService;
    }

    public async Task<MenuDto?> GetByIdAsync(int id, string? languageCode = null)
    {
        var menu = await _unitOfWork.Menus.GetByIdAsync(id, m => m.Translations, m => m.Items);
        if (menu == null) return null;

        var dto = _mapper.Map<MenuDto>(menu);
        await SetTranslationAsync(dto, languageCode);
        
        return dto;
    }

    public async Task<MenuDto?> GetByNameAsync(string name, string languageCode)
    {
        var menu = await _unitOfWork.Menus.FirstOrDefaultAsync(m => m.Name == name, m => m.Translations, m => m.Items.Where(i => i.IsActive).OrderBy(i => i.DisplayOrder));
        if (menu == null) return null;

        var dto = _mapper.Map<MenuDto>(menu);
        await SetTranslationAsync(dto, languageCode);
        
        return dto;
    }

    public async Task<IReadOnlyList<MenuDto>> GetAllAsync(string? languageCode = null)
    {
        var menus = await _unitOfWork.Menus.GetAllAsync(m => m.IsActive, q => q.OrderBy(m => m.Name), m => m.Translations, m => m.Items);
        var dtos = _mapper.Map<IReadOnlyList<MenuDto>>(menus);
        await SetTranslationsAsync(dtos, languageCode);
        return dtos;
    }

    public async Task<MenuDto> CreateAsync(CreateMenuDto dto)
    {
        var menu = _mapper.Map<Menu>(dto);
        menu.CreatedAt = DateTime.UtcNow;
        
        foreach (var transDto in dto.Translations)
        {
            var translation = _mapper.Map<MenuTranslation>(transDto);
            translation.Menu = menu;
            menu.Translations.Add(translation);
        }

        await _unitOfWork.Menus.AddAsync(menu);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<MenuDto>(menu);
    }

    public async Task<MenuDto> UpdateAsync(int id, UpdateMenuDto dto)
    {
        var menu = await _unitOfWork.Menus.GetByIdAsync(id, m => m.Translations);
        if (menu == null)
        {
            throw new KeyNotFoundException($"Menu with id {id} not found.");
        }

        _mapper.Map(dto, menu);
        menu.UpdatedAt = DateTime.UtcNow;

        await UpdateTranslationsAsync(menu.Translations, dto.Translations, (t, d) => _mapper.Map(d, t));

        await _unitOfWork.Menus.UpdateAsync(menu);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<MenuDto>(menu);
    }

    public async Task DeleteAsync(int id)
    {
        var menu = await _unitOfWork.Menus.GetByIdAsync(id);
        if (menu == null)
        {
            throw new KeyNotFoundException($"Menu with id {id} not found.");
        }

        // Check if menu has items
        var hasItems = await _unitOfWork.MenuItems.ExistsAsync(i => i.MenuId == id);
        if (hasItems)
        {
            throw new InvalidOperationException("Cannot delete menu that has items. Delete items first.");
        }

        await _unitOfWork.Menus.DeleteAsync(menu);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<MenuItemDto>> GetMenuItemsAsync(string menuName, string languageCode)
    {
        var menu = await _unitOfWork.Menus.FirstOrDefaultAsync(m => m.Name == menuName);
        if (menu == null) return new List<MenuItemDto>();

        var items = await _unitOfWork.MenuItems.GetAllAsync(
            i => i.MenuId == menu.Id && i.IsActive,
            q => q.OrderBy(i => i.DisplayOrder),
            i => i.Translations,
            i => i.Children.Where(c => c.IsActive).OrderBy(c => c.DisplayOrder));

        var dtos = _mapper.Map<IReadOnlyList<MenuItemDto>>(items);
        await SetItemTranslationsAsync(dtos, languageCode);
        
        return dtos;
    }

    public async Task<MenuItemDto> CreateMenuItemAsync(CreateMenuItemDto dto)
    {
        var menuItem = _mapper.Map<MenuItem>(dto);
        menuItem.CreatedAt = DateTime.UtcNow;
        
        foreach (var transDto in dto.Translations)
        {
            var translation = _mapper.Map<MenuItemTranslation>(transDto);
            translation.MenuItem = menuItem;
            menuItem.Translations.Add(translation);
        }

        await _unitOfWork.MenuItems.AddAsync(menuItem);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<MenuItemDto>(menuItem);
    }

    public async Task<MenuItemDto> UpdateMenuItemAsync(int id, UpdateMenuItemDto dto)
    {
        var menuItem = await _unitOfWork.MenuItems.GetByIdAsync(id, i => i.Translations);
        if (menuItem == null)
        {
            throw new KeyNotFoundException($"Menu item with id {id} not found.");
        }

        _mapper.Map(dto, menuItem);
        menuItem.UpdatedAt = DateTime.UtcNow;

        await UpdateTranslationsAsync(menuItem.Translations, dto.Translations, (t, d) => _mapper.Map(d, t));

        await _unitOfWork.MenuItems.UpdateAsync(menuItem);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<MenuItemDto>(menuItem);
    }

    public async Task DeleteMenuItemAsync(int id)
    {
        var menuItem = await _unitOfWork.MenuItems.GetByIdAsync(id);
        if (menuItem == null)
        {
            throw new KeyNotFoundException($"Menu item with id {id} not found.");
        }

        // Check for children
        var hasChildren = await _unitOfWork.MenuItems.ExistsAsync(i => i.ParentId == id);
        if (hasChildren)
        {
            throw new InvalidOperationException("Cannot delete menu item that has children. Delete children first.");
        }

        await _unitOfWork.MenuItems.DeleteAsync(menuItem);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ReorderMenuItemsAsync(int menuId, List<ReorderItemDto> items)
    {
        foreach (var item in items)
        {
            var menuItem = await _unitOfWork.MenuItems.GetByIdAsync(item.Id);
            if (menuItem != null && menuItem.MenuId == menuId)
            {
                menuItem.DisplayOrder = item.DisplayOrder;
                menuItem.ParentId = item.ParentId;
                menuItem.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.MenuItems.UpdateAsync(menuItem);
            }
        }
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<MenuItemDto>> GetMenuTreeAsync(string menuName, string languageCode)
    {
        var menu = await _unitOfWork.Menus.FirstOrDefaultAsync(m => m.Name == menuName);
        if (menu == null) return new List<MenuItemDto>();

        var items = await _unitOfWork.MenuItems.GetAllAsync(
            i => i.MenuId == menu.Id && i.IsActive && i.ParentId == null,
            q => q.OrderBy(i => i.DisplayOrder),
            i => i.Translations,
            i => i.Children.Where(c => c.IsActive).OrderBy(c => c.DisplayOrder));

        var dtos = _mapper.Map<IReadOnlyList<MenuItemDto>>(items);
        await SetItemTranslationsAsync(dtos, languageCode);
        
        return dtos;
    }

    private async Task SetTranslationAsync(MenuDto dto, string? languageCode)
    {
        if (!string.IsNullOrEmpty(languageCode))
        {
            var language = await _languageService.GetByCodeAsync(languageCode);
            if (language != null)
            {
                dto.Translation = dto.Translations.FirstOrDefault(t => t.LanguageId == language.Id);
                
                foreach (var item in dto.Items)
                {
                    await SetItemTranslationAsync(item, language.Id);
                }
            }
        }
    }

    private async Task SetItemTranslationAsync(MenuItemDto dto, int languageId)
    {
        dto.Translation = dto.Translations.FirstOrDefault(t => t.LanguageId == languageId);
        
        foreach (var child in dto.Children)
        {
            await SetItemTranslationAsync(child, languageId);
        }
    }

    private async Task SetTranslationsAsync(IReadOnlyList<MenuDto> dtos, string? languageCode)
    {
        if (!string.IsNullOrEmpty(languageCode))
        {
            var language = await _languageService.GetByCodeAsync(languageCode);
            if (language != null)
            {
                foreach (var dto in dtos)
                {
                    await SetTranslationAsync(dto, languageCode);
                }
            }
        }
    }

    private async Task SetItemTranslationsAsync(IReadOnlyList<MenuItemDto> dtos, string? languageCode)
    {
        if (!string.IsNullOrEmpty(languageCode))
        {
            var language = await _languageService.GetByCodeAsync(languageCode);
            if (language != null)
            {
                foreach (var dto in dtos)
                {
                    await SetItemTranslationAsync(dto, language.Id);
                }
            }
        }
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