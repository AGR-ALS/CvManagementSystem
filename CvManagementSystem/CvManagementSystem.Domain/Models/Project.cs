namespace UserService.Domain.Models;

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public List<Technology> Technologies { get; set; } = [];
    public Guid UserId { get; set; }
    public uint Version { get; set; }
}