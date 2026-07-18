using CorporateWebsite.Web.Services;
using CorporateWebsite.Core.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace CorporateWebsite.Web.Services;

public interface IImageUrlService
{
    string GetImageUrl(string? relativePath, int? width = null, int? height = null, string format = "webp", int quality = 80);
    string GetOptimizedUrl(string? relativePath, int width, int height, string format = "webp", int quality = 80);
    string GetThumbnailUrl(string? relativePath, int width, int height, string format = "webp", int quality = 80);
    string GetPlaceholderUrl(int width, int height, string text = "", string bgColor = "e2e8f0", string textColor = "64748b");
    string GetResponsiveImageSet(string? relativePath, int[] widths, string format = "webp", int quality = 80);
    IHtmlContent RenderPictureElement(string? relativePath, string alt, string cssClass = "", int[]? widths = null, string format = "webp", int quality = 80, Dictionary<int, string>? sources = null);
}

public class ImageUrlService : IImageUrlService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;
    private readonly bool _useOptimization;
    private readonly string _cdnUrl;

    public ImageUrlService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
        _useOptimization = configuration.GetValue<bool>("ImageOptimization:Enabled", true);
        _cdnUrl = configuration["ImageOptimization:CdnUrl"] ?? "";
    }

    public string GetImageUrl(string? relativePath, int? width = null, int? height = null, string format = "webp", int quality = 80)
    {
        if (string.IsNullOrEmpty(relativePath))
        {
            return GetPlaceholderUrl(width ?? 400, height ?? 300);
        }

        // If already absolute URL
        if (relativePath.StartsWith("http") || relativePath.StartsWith("//"))
        {
            return relativePath;
        }

        // Ensure path starts with /
        var path = relativePath.StartsWith("/") ? relativePath : $"/{relativePath}";

        // If optimization is disabled or no dimensions specified, return original
        if (!_useOptimization || (!width.HasValue && !height.HasValue))
        {
            return GetAbsoluteUrl(path);
        }

        // Return optimized URL
        return GetOptimizedUrl(path, width ?? 0, height ?? 0, format, quality);
    }

    public string GetOptimizedUrl(string? relativePath, int width, int height, string format = "webp", int quality = 80)
    {
        if (string.IsNullOrEmpty(relativePath))
        {
            return GetPlaceholderUrl(width, height);
        }

        if (relativePath.StartsWith("http") || relativePath.StartsWith("//"))
        {
            return relativePath;
        }

        var path = relativePath.StartsWith("/") ? relativePath : $"/{relativePath}";
        var baseUrl = _cdnUrl != "" ? _cdnUrl : GetBaseUrl();
        
        // Use query parameters for optimization (handled by middleware or image server)
        var queryParams = new List<string>();
        if (width > 0) queryParams.Add($"w={width}");
        if (height > 0) queryParams.Add($"h={height}");
        queryParams.Add($"f={format}");
        queryParams.Add($"q={quality}");
        
        return $"{baseUrl}{path}?{string.Join("&", queryParams)}";
    }

    public string GetThumbnailUrl(string? relativePath, int width, int height, string format = "webp", int quality = 80)
    {
        return GetOptimizedUrl(relativePath, width, height, format, quality);
    }

    public string GetPlaceholderUrl(int width, int height, string text = "", string bgColor = "e2e8f0", textColor = "64748b")
    {
        var encodedText = Uri.EscapeDataString(text);
        return $"https://via.placeholder.com/{width}x{height}/{bgColor}/{textColor}?text={encodedText}";
    }

    public string GetResponsiveImageSet(string? relativePath, int[] widths, string format = "webp", int quality = 80)
    {
        if (string.IsNullOrEmpty(relativePath) || !widths.Any())
        {
            return "";
        }

        var baseUrl = _cdnUrl != "" ? _cdnUrl : GetBaseUrl();
        var path = relativePath.StartsWith("/") ? relativePath : $"/{relativePath}";
        
        var srcSet = widths.Select(w => 
        {
            var url = $"{baseUrl}{path}?w={w}&f={format}&q={quality}";
            return $"{url} {w}w";
        });

        return string.Join(", ", srcSet);
    }

    public IHtmlContent RenderPictureElement(string? relativePath, string alt, string cssClass = "", int[]? widths = null, string format = "webp", int quality = 80, Dictionary<int, string>? sources = null)
    {
        if (string.IsNullOrEmpty(relativePath))
        {
            var placeholder = GetPlaceholderUrl(widths?.FirstOrDefault() ?? 400, 300);
            return new HtmlString($"<img src=\"{placeholder}\" alt=\"{System.Net.WebUtility.HtmlEncode(alt)}\" class=\"{cssClass}\" loading=\"lazy\" />");
        }

        var path = relativePath.StartsWith("/") ? relativePath : $"/{relativePath}";
        var baseUrl = _cdnUrl != "" ? _cdnUrl : GetBaseUrl();
        var defaultWidth = widths?.FirstOrDefault() ?? 800;
        
        var sb = new StringBuilder();
        
        sb.AppendLine($"<picture class=\"{cssClass}\">");

        // WebP source
        if (format == "webp" || format == "avif")
        {
            var webpSrcSet = GetResponsiveImageSet(path, widths ?? new[] { defaultWidth }, format, quality);
            if (!string.IsNullOrEmpty(webpSrcSet))
            {
                sb.AppendLine($"  <source type=\"image/{format}\" srcset=\"{webpSrcSet}\" />");
            }
        }

        // Fallback sources (different formats)
        if (sources != null)
        {
            foreach (var source in sources)
            {
                var srcSet = GetResponsiveImageSet(path, widths ?? new[] { defaultWidth }, source.Value, quality);
                if (!string.IsNullOrEmpty(srcSet))
                {
                    sb.AppendLine($"  <source type=\"image/{source.Value}\" srcset=\"{srcSet}\" />");
                }
            }
        }

        // Fallback img
        var fallbackUrl = $"{baseUrl}{path}";
        var imgClass = string.IsNullOrEmpty(cssClass) ? "" : $" class=\"{cssClass}\"";
        
        sb.AppendLine($"  <img src=\"{fallbackUrl}\" alt=\"{System.Net.WebUtility.HtmlEncode(alt)}\"{imgClass} loading=\"lazy\" decoding=\"async\" />");
        sb.AppendLine("</picture>");

        return new HtmlString(sb.ToString());
    }

    private string GetAbsoluteUrl(string relativePath)
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        if (request == null) return relativePath;
        
        var baseUrl = _cdnUrl != "" ? _cdnUrl : $"{request.Scheme}://{request.Host}";
        return $"{baseUrl}{relativePath}";
    }

    private string GetBaseUrl()
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        if (request == null) return "";
        
        return _cdnUrl != "" ? _cdnUrl : $"{request.Scheme}://{request.Host}";
    }
}