using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Formats.Avif;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using Microsoft.Extensions.Configuration;

namespace CorporateWebsite.Infrastructure.Services;

public interface IImageOptimizationService
{
    Task<byte[]> OptimizeAsync(Stream input, string format = "webp", int quality = 80, int? maxWidth = null, int? maxHeight = null);
    Task<byte[]> GenerateThumbnailAsync(Stream input, int width, int height, string format = "webp", int quality = 80);
    Task<string> GetOptimizedUrlAsync(string originalUrl, int width, int height, string format = "webp", int quality = 80);
    Task<bool> IsImageFileAsync(string contentType);
    Task<(int Width, int Height)> GetDimensionsAsync(Stream input);
}

public class ImageOptimizationService : IImageOptimizationService
{
    private readonly IConfiguration _configuration;

    public ImageOptimizationService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<byte[]> OptimizeAsync(Stream input, string format = "webp", int quality = 80, int? maxWidth = null, int? maxHeight = null)
    {
        input.Position = 0;
        
        using var image = await Image.LoadAsync(input);
        
        // Auto-rotate based on EXIF orientation
        image.Mutate(x => x.AutoOrient());

        // Resize if needed
        if (maxWidth.HasValue || maxHeight.HasValue)
        {
            var options = new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(maxWidth ?? int.MaxValue, maxHeight ?? int.MaxValue)
            };
            image.Mutate(x => x.Resize(options));
        }

        // Strip metadata for smaller size
        image.Metadata.ExifProfile = null;
        image.Metadata.XmpProfile = null;

        var output = new MemoryStream();
        
        switch (format.ToLower())
        {
            case "webp":
                var webpEncoder = new WebpEncoder
                {
                    Quality = quality,
                    Method = 6,
                    Lossless = false
                };
                await image.SaveAsync(output, webpEncoder);
                break;
            
            case "avif":
                var avifEncoder = new AvifEncoder
                {
                    Quality = quality,
                    Speed = 6
                };
                await image.SaveAsync(output, avifEncoder);
                break;
            
            case "jpeg":
            case "jpg":
                var jpegEncoder = new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder
                {
                    Quality = quality
                };
                await image.SaveAsync(output, jpegEncoder);
                break;
            
            case "png":
                var pngEncoder = new SixLabors.ImageSharp.Formats.Png.PngEncoder
                {
                    CompressionLevel = SixLabors.ImageSharp.Formats.Png.PngCompressionLevel.BestCompression
                };
                await image.SaveAsync(output, pngEncoder);
                break;
            
            default:
                var defaultEncoder = new WebpEncoder { Quality = quality };
                await image.SaveAsync(output, defaultEncoder);
                break;
        }

        return output.ToArray();
    }

    public async Task<byte[]> GenerateThumbnailAsync(Stream input, int width, int height, string format = "webp", int quality = 80)
    {
        input.Position = 0;
        
        using var image = await Image.LoadAsync(input);
        image.Mutate(x => x.AutoOrient());

        var options = new ResizeOptions
        {
            Mode = ResizeMode.Crop,
            Size = new Size(width, height),
            Position = AnchorPositionMode.Center
        };
        image.Mutate(x => x.Resize(options));

        image.Metadata.ExifProfile = null;
        image.Metadata.XmpProfile = null;

        var output = new MemoryStream();
        
        switch (format.ToLower())
        {
            case "webp":
                await image.SaveAsync(output, new WebpEncoder { Quality = quality });
                break;
            case "avif":
                await image.SaveAsync(output, new AvifEncoder { Quality = quality });
                break;
            default:
                await image.SaveAsync(output, new WebpEncoder { Quality = quality });
                break;
        }

        return output.ToArray();
    }

    public async Task<string> GetOptimizedUrlAsync(string originalUrl, int width, int height, string format = "webp", int quality = 80)
    {
        var uri = new Uri(originalUrl);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        query["w"] = width.ToString();
        query["h"] = height.ToString();
        query["f"] = format;
        query["q"] = quality.ToString();
        
        var builder = new UriBuilder(uri)
        {
            Query = query.ToString()
        };
        
        return builder.ToString();
    }

    public async Task<bool> IsImageFileAsync(string contentType)
    {
        return contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<(int Width, int Height)> GetDimensionsAsync(Stream input)
    {
        input.Position = 0;
        
        using var image = await Image.LoadAsync(input);
        return (image.Width, image.Height);
    }
}