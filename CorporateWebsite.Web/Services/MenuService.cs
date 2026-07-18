using CorporateWebsite.Web.Services;
using CorporateWebsite.Core.DTOs;
using CorporateWebsite.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace CorporateWebsite.Web.Services;

public interface IMenuService
{
    Task<IReadOnlyList<MenuItemDto>> GetMenuAsync(string menuName);
    Task<IReadOnlyList<MenuItemDto>> GetMenuTreeAsync(string menuName);
    IHtmlContent RenderMenu(string menuName, string cssClass = "nav", bool dropdown = true);
    IHtmlContent RenderMobileMenu(string menuName, string cssClass = "mobile-nav");
    void InvalidateCache(string menuName);
}

public class MenuService : IMenuService
{
    private readonly CorporateWebsite.Application.Interfaces.IMenuService _menuService;
    private readonly IMemoryCache _cache;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILocalizationService _localizationService;

    public MenuService(
        CorporateWebsite.Application.Interfaces.IMenuService menuService,
        IMemoryCache cache,
        IHttpContextAccessor httpContextAccessor,
        ILocalizationService localizationService)
    {
        _menuService = menuService;
        _cache = cache;
        _httpContextAccessor = httpContextAccessor;
        _localizationService = localizationService;
    }

    public async Task<IReadOnlyList<MenuItemDto>> GetMenuAsync(string menuName)
    {
        var culture = _localizationService.CurrentCulture;
        var cacheKey = $"menu_{menuName}_{culture}";
        
        if (!_cache.TryGetValue(cacheKey, out IReadOnlyList<MenuItemDto>? menuItems))
        {
            menuItems = await _menuService.GetMenuItemsAsync(menuName, culture);
            _cache.Set(cacheKey, menuItems, TimeSpan.FromMinutes(30));
        }

        return menuItems!;
    }

    public async Task<IReadOnlyList<MenuItemDto>> GetMenuTreeAsync(string menuName)
    {
        var culture = _localizationService.CurrentCulture;
        var cacheKey = $"menu_tree_{menuName}_{culture}";
        
        if (!_cache.TryGetValue(cacheKey, out IReadOnlyList<MenuItemDto>? menuItems))
        {
            menuItems = await _menuService.GetMenuTreeAsync(menuName, culture);
            _cache.Set(cacheKey, menuItems, TimeSpan.FromMinutes(30));
        }

        return menuItems!;
    }

    public IHtmlContent RenderMenu(string menuName, string cssClass = "nav", bool dropdown = true)
    {
        var menuItems = GetMenuAsync(menuName).GetAwaiter().GetResult();
        return RenderMenuItems(menuItems, cssClass, dropdown, 0);
    }

    public IHtmlContent RenderMobileMenu(string menuName, string cssClass = "mobile-nav")
    {
        var menuItems = GetMenuTreeAsync(menuName).GetAwaiter().GetResult();
        return RenderMenuItems(menuItems, cssClass, true, 0, true);
    }

    public void InvalidateCache(string menuName)
    {
        var cultures = new[] { "fa", "en", "ar" };
        foreach (var culture in cultures)
        {
            _cache.Remove($"menu_{menuName}_{culture}");
            _cache.Remove($"menu_tree_{menuName}_{culture}");
        }
    }

    private IHtmlContent RenderMenuItems(IReadOnlyList<MenuItemDto> items, string cssClass, bool dropdown, int level, bool isMobile = false)
    {
        if (!items.Any()) return new HtmlString("");

        var sb = new StringBuilder();
        var culture = _localizationService.CurrentCulture;
        var isRtl = _localizationService.IsRtl;
        var ulClass = level == 0 ? cssClass : (dropdown ? "dropdown-menu" : "submenu");
        
        if (isMobile)
        {
            ulClass = "mobile-menu";
        }

        sb.AppendLine($"<ul class=\"{ulClass}\" role=\"{(level == 0 ? "menubar" : "menu")}\">");

        foreach (var item in items)
        {
            var translation = item.Translation;
            var title = translation?.Title ?? "";
            var hasChildren = item.Children?.Any() == true;
            var itemCss = new List<string> { "menu-item" };
            
            if (hasChildren)
            {
                itemCss.Add("has-children");
                if (dropdown) itemCss.Add("dropdown");
            }
            
            if (!string.IsNullOrEmpty(item.CssClass))
            {
                itemCss.AddRange(item.CssClass.Split(' ', StringSplitOptions.RemoveEmptyEntries));
            }

            sb.AppendLine($"  <li class=\"{string.Join(" ", itemCss)}\" role=\"{(hasChildren ? "none" : "menuitem")}\">");

            var url = GetItemUrl(item);
            var target = item.Target ?? "_self";
            
            if (hasChildren && dropdown)
            {
                sb.AppendLine($"    <a href=\"{url}\" class=\"dropdown-toggle\" data-bs-toggle=\"dropdown\" aria-expanded=\"false\" role=\"button\" target=\"{target}\">");
                sb.AppendLine($"      {System.Net.WebUtility.HtmlEncode(title)}");
                if (!isMobile)
                {
                    sb.AppendLine("      <i class=\"fas fa-chevron-down ms-1\"></i>");
                }
                sb.AppendLine("    </a>");
                
                if (item.Children?.Any() == true)
                {
                    sb.Append(RenderMenuItems(item.Children, cssClass, dropdown, level + 1, isMobile).ToString());
                }
            }
            else
            {
                var iconHtml = !string.IsNullOrEmpty(item.Icon) ? $"<i class=\"{item.Icon} me-1\"></i>" : "";
                sb.AppendLine($"    <a href=\"{url}\" target=\"{target}\" class=\"menu-link\">");
                sb.AppendLine($"      {iconHtml}{System.Net.WebUtility.HtmlEncode(title)}");
                sb.AppendLine("    </a>");
            }

            sb.AppendLine("  </li>");
        }

        sb.AppendLine("</ul>");

        return new HtmlString(sb.ToString());
    }

    private string GetItemUrl(MenuItemDto item)
    {
        var culture = _localizationService.CurrentCulture;
        
        if (!string.IsNullOrEmpty(item.CustomUrl))
        {
            return item.CustomUrl.StartsWith("http") ? item.CustomUrl : $"/{culture}{item.CustomUrl}";
        }
        
        if (item.PageId.HasValue && item.Page != null)
        {
            var translation = item.Page.Translation;
            if (translation != null)
            {
                return $"/{culture}/{translation.Slug}";
            }
        }
        
        if (item.ServiceId.HasValue && item.Service != null)
        {
            var translation = item.Service.Translation;
            if (translation != null)
            {
                return $"/{culture}/services/{translation.Slug}";
            }
        }
        
        if (item.ProjectId.HasValue && item.Project != null)
        {
            var translation = item.Project.Translation;
            if (translation != null)
            {
                return $"/{culture}/projects/{translation.Slug}";
            }
        }
        
        if (item.NewsCategoryId.HasValue && item.NewsCategory != null)
        {
            var translation = item.NewsCategory.Translation;
            if (translation != null)
            {
                return $"/{culture}/news/category/{translation.Slug}";
            }
        }

        return "#";
    }
}