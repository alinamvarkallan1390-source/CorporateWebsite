namespace CorporateWebsite.Core.Entities;

public class NewsComment : BaseEntity
{
    public int NewsId { get; set; }
    public int? ParentId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorEmail { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsApproved { get; set; } = false;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    
    public News News { get; set; } = null!;
    public NewsComment? Parent { get; set; }
    public ICollection<NewsComment> Replies { get; set; } = new List<NewsComment>();
}