using CorporateWebsite.Core.DTOs;
using CorporateWebsite.Core.Entities;
using CorporateWebsite.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CorporateWebsite.Infrastructure.Services;

public class SearchService : ISearchService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILanguageService _languageService;

    public SearchService(IUnitOfWork unitOfWork, ILanguageService languageService)
    {
        _unitOfWork = unitOfWork;
        _languageService = languageService;
    }

    public async Task<SearchResultDto> SearchAsync(string query, string languageCode, int page = 1, int pageSize = 10, string[]? types = null)
    {
        var startTime = DateTime.UtcNow;
        var language = await _languageService.GetByCodeAsync(languageCode);
        
        if (language == null)
        {
            return new SearchResultDto
            {
                Query = query,
                TotalResults = 0,
                Page = page,
                PageSize = pageSize,
                SearchTimeMs = 0,
                Items = new List<SearchResultItemDto>()
            };
        }

        var searchTerms = query.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var typesToSearch = types ?? new[] { "pages", "services", "projects", "news" };

        var allResults = new List<SearchResultItemDto>();

        if (typesToSearch.Contains("pages"))
        {
            var pages = await SearchPagesAsync(searchTerms, language.Id);
            allResults.AddRange(pages);
        }

        if (typesToSearch.Contains("services"))
        {
            var services = await SearchServicesAsync(searchTerms, language.Id);
            allResults.AddRange(services);
        }

        if (typesToSearch.Contains("projects"))
        {
            var projects = await SearchProjectsAsync(searchTerms, language.Id);
            allResults.AddRange(projects);
        }

        if (typesToSearch.Contains("news"))
        {
            var news = await SearchNewsAsync(searchTerms, language.Id);
            allResults.AddRange(news);
        }

        // Sort by relevance score
        allResults = allResults.OrderByDescending(r => r.Score).ToList();

        // Pagination
        var totalResults = allResults.Count;
        var pagedResults = allResults.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return new SearchResultDto
        {
            Query = query,
            TotalResults = totalResults,
            Page = page,
            PageSize = pageSize,
            SearchTimeMs = (DateTime.UtcNow - startTime).TotalMilliseconds,
            Items = pagedResults
        };
    }

    private async Task<List<SearchResultItemDto>> SearchPagesAsync(string[] searchTerms, int languageId)
    {
        var pages = await _unitOfWork.PageTranslations.GetAllAsync(
            t => t.LanguageId == languageId && t.Page.IsPublished &&
                 searchTerms.Any(st => t.Title.ToLower().Contains(st) || t.Content.ToLower().Contains(st)),
            q => q.Include(t => t.Page));

        return pages.Select(p => new SearchResultItemDto
        {
            EntityType = "page",
            EntityId = p.PageId,
            Title = p.Title,
            Description = TruncateText(p.ShortDescription ?? p.Content, 200),
            Url = $"/{p.Language.Code}/{p.Slug}",
            PublishedAt = p.Page.PublishedAt,
            Score = CalculateScore(p.Title, p.Content, searchTerms),
            Highlights = GetHighlights(p.Title, p.Content, searchTerms)
        }).ToList();
    }

    private async Task<List<SearchResultItemDto>> SearchServicesAsync(string[] searchTerms, int languageId)
    {
        var services = await _unitOfWork.ServiceTranslations.GetAllAsync(
            t => t.LanguageId == languageId && t.Service.IsPublished &&
                 searchTerms.Any(st => t.Title.ToLower().Contains(st) || t.Content.ToLower().Contains(st)),
            q => q.Include(t => t.Service));

        return services.Select(s => new SearchResultItemDto
        {
            EntityType = "service",
            EntityId = s.ServiceId,
            Title = s.Title,
            Description = TruncateText(s.ShortDescription ?? s.Content, 200),
            Url = $"/{s.Language.Code}/services/{s.Slug}",
            ImageUrl = s.Service.ImageUrl,
            PublishedAt = s.Service.PublishedAt,
            Score = CalculateScore(s.Title, s.Content, searchTerms),
            Highlights = GetHighlights(s.Title, s.Content, searchTerms)
        }).ToList();
    }

    private async Task<List<SearchResultItemDto>> SearchProjectsAsync(string[] searchTerms, int languageId)
    {
        var projects = await _unitOfWork.ProjectTranslations.GetAllAsync(
            t => t.LanguageId == languageId && t.Project.IsPublished &&
                 searchTerms.Any(st => t.Title.ToLower().Contains(st) || t.Content.ToLower().Contains(st)),
            q => q.Include(t => t.Project));

        return projects.Select(p => new SearchResultItemDto
        {
            EntityType = "project",
            EntityId = p.ProjectId,
            Title = p.Title,
            Description = TruncateText(p.ShortDescription ?? p.Content, 200),
            Url = $"/{p.Language.Code}/projects/{p.Slug}",
            ImageUrl = p.Project.MainImageUrl,
            PublishedAt = p.Project.PublishedAt,
            Score = CalculateScore(p.Title, p.Content, searchTerms),
            Highlights = GetHighlights(p.Title, p.Content, searchTerms)
        }).ToList();
    }

    private async Task<List<SearchResultItemDto>> SearchNewsAsync(string[] searchTerms, int languageId)
    {
        var news = await _unitOfWork.NewsTranslations.GetAllAsync(
            t => t.LanguageId == languageId && t.News.IsPublished &&
                 searchTerms.Any(st => t.Title.ToLower().Contains(st) || t.Content.ToLower().Contains(st)),
            q => q.Include(t => t.News));

        return news.Select(n => new SearchResultItemDto
        {
            EntityType = "news",
            EntityId = n.NewsId,
            Title = n.Title,
            Description = TruncateText(n.ShortDescription ?? n.Content, 200),
            Url = $"/{n.Language.Code}/news/{n.Slug}",
            ImageUrl = n.News.MainImageUrl,
            PublishedAt = n.News.PublishedAt,
            Score = CalculateScore(n.Title, n.Content, searchTerms),
            Highlights = GetHighlights(n.Title, n.Content, searchTerms)
        }).ToList();
    }

    public async Task ReindexAsync()
    {
        // For database-based search, no reindexing needed
        await Task.CompletedTask;
    }

    public async Task IndexEntityAsync(string entityType, int entityId)
    {
        // For database-based search, no indexing needed
        await Task.CompletedTask;
    }

    public async Task RemoveFromIndexAsync(string entityType, int entityId)
    {
        // For database-based search, no removal needed
        await Task.CompletedTask;
    }

    public async Task<IReadOnlyList<SearchSuggestionDto>> GetSuggestionsAsync(string query, string languageCode, int count = 10)
    {
        var language = await _languageService.GetByCodeAsync(languageCode);
        if (language == null) return new List<SearchSuggestionDto>();

        var searchTerm = query.ToLower();
        var suggestions = new List<SearchSuggestionDto>();

        // Get suggestions from pages
        var pages = await _unitOfWork.PageTranslations.GetAllAsync(
            t => t.LanguageId == language.Id && t.Page.IsPublished && t.Title.ToLower().Contains(searchTerm),
            q => q.Take(count / 4));
        
        suggestions.AddRange(pages.Select(p => new SearchSuggestionDto
        {
            Text = p.Title,
            Type = "page",
            Count = 1
        }));

        // Get suggestions from services
        var services = await _unitOfWork.ServiceTranslations.GetAllAsync(
            t => t.LanguageId == language.Id && t.Service.IsPublished && t.Title.ToLower().Contains(searchTerm),
            q => q.Take(count / 4));
        
        suggestions.AddRange(services.Select(s => new SearchSuggestionDto
        {
            Text = s.Title,
            Type = "service",
            Count = 1
        }));

        // Get suggestions from projects
        var projects = await _unitOfWork.ProjectTranslations.GetAllAsync(
            t => t.LanguageId == language.Id && t.Project.IsPublished && t.Title.ToLower().Contains(searchTerm),
            q => q.Take(count / 4));
        
        suggestions.AddRange(projects.Select(p => new SearchSuggestionDto
        {
            Text = p.Title,
            Type = "project",
            Count = 1
        }));

        // Get suggestions from news
        var news = await _unitOfWork.NewsTranslations.GetAllAsync(
            t => t.LanguageId == language.Id && t.News.IsPublished && t.Title.ToLower().Contains(searchTerm),
            q => q.Take(count / 4));
        
        suggestions.AddRange(news.Select(n => new SearchSuggestionDto
        {
            Text = n.Title,
            Type = "news",
            Count = 1
        }));

        return suggestions.Take(count).ToList();
    }

    private double CalculateScore(string title, string content, string[] searchTerms)
    {
        var score = 0.0;
        var titleLower = title.ToLower();
        var contentLower = content.ToLower();

        foreach (var term in searchTerms)
        {
            // Title matches are worth more
            var titleMatches = CountOccurrences(titleLower, term);
            score += titleMatches * 10;

            // Content matches
            var contentMatches = CountOccurrences(contentLower, term);
            score += contentMatches * 1;

            // Exact phrase match bonus
            if (titleLower.Contains(term))
            {
                score += 5;
            }
        }

        return score;
    }

    private int CountOccurrences(string text, string term)
    {
        var count = 0;
        var index = 0;
        while ((index = text.IndexOf(term, index, StringComparison.Ordinal)) != -1)
        {
            count++;
            index += term.Length;
        }
        return count;
    }

    private string TruncateText(string text, int maxLength)
    {
        if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
        {
            return text ?? string.Empty;
        }
        return text.Substring(0, maxLength).TrimEnd() + "...";
    }

    private Dictionary<string, string> GetHighlights(string title, string content, string[] searchTerms)
    {
        var highlights = new Dictionary<string, string>();
        
        var highlightedTitle = HighlightText(title, searchTerms);
        var highlightedContent = HighlightText(content, searchTerms);
        
        if (highlightedTitle != title)
        {
            highlights["title"] = highlightedTitle;
        }
        
        if (highlightedContent != content)
        {
            highlights["content"] = highlightedContent.Length > 300 ? highlightedContent.Substring(0, 300) + "..." : highlightedContent;
        }

        return highlights;
    }

    private string HighlightText(string text, string[] searchTerms)
    {
        var result = text;
        foreach (var term in searchTerms)
        {
            var regex = new Regex(Regex.Escape(term), RegexOptions.IgnoreCase);
            result = regex.Replace(result, "<mark>$0</mark>");
        }
        return result;
    }
}