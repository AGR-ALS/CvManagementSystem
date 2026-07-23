using UserService.Domain.Models;

namespace UserService.DataAccess.Entitites;

public class CvProject
{
    public Guid CvId { get; set; }
    public Cv Cv { get; set; } = null!;

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}