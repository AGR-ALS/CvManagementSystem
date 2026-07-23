using UserService.Domain.Models;

namespace UserService.DataAccess.Entitites;

public class ProjectTechnology
{
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public string TechnologyId { get; set; } = null!;
    public Technology Technology { get; set; } = null!;
}