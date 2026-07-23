using UserService.Domain.Models.Attributes;

namespace UserService.Domain.Models;

public class Position
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public ExpertiseLevel ExpertiseLevel { get; set; }
    public List<AccessRule> AccessRules { get; set; } = null!;
    public List<Technology> Technologies { get; set; } = null!;
    public uint MaxProjects { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool Restricted { get; set; }
    public uint Version { get; set; }
}