namespace CorporateWebsite.Core.Entities;

public class RelatedNews : BaseEntity
{
    public int NewsId { get; set; }
    public int RelatedNewsId { get; set; }
    public int DisplayOrder { get; set; }
    
    public News News { get; set; } = null!;
    public News RelatedNewsItem { get; set; } = null!;
}