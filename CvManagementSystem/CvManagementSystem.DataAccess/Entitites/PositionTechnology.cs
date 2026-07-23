using UserService.Domain.Models;

namespace UserService.DataAccess.Entitites;

public class PositionTechnology
{
    public Guid PositionId { get; set; }
    public Position Position { get; set; } = null!;

    public string TechnologyId { get; set; } = null!;
    public Technology Technology { get; set; } = null!;
}