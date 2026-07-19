namespace CorporateWebsite.Core.Entities;

public class ProjectTag : BaseEntity
{
    public int ProjectId { get; set; }
    public int TagId { get; set; }
    
    public Project Project { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}