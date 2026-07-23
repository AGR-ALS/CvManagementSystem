using UserService.Domain.Models.Attributes;

namespace UserService.Domain.Models;

public class Cv
{
    public Guid Id { get; set; }
    public User User { get; set; } = null!;
    public Guid UserId { get; set; }
    public Position Position { get; set; } = null!;
    public Guid PositionId { get; set; }
    public List<Project> Projects { get; set; } = null!;
    public uint Likes { get; set; }
    public bool Published { get; set; }
    public uint Version { get; set; }
}