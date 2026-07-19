namespace CorporateWebsite.Core.Entities;

public class NewsTag : BaseEntity
{
    public int NewsId { get; set; }
    public int TagId { get; set; }
    
    public News News { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}