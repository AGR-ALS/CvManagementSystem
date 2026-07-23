using UserService.Domain.Models.Attributes;

namespace UserService.Domain.Models;

public class User
{
    public Guid Id { get; set; }
    public ProfileData ProfileData { get; set; } = null!;
    public Role Role { get; set; } = null!;
    public Guid? RoleId { get; set; }
    public List<Project> Projects { get; set; }  = [];
    public List<UserAttributeValue> Attributes { get; set; } = null!;
    public bool IsBlocked { get; set; }
    public string Email { get; set; } = null!;
    public string? PasswordHash { get; set; } = null;
    public uint Version { get; set; }
    public bool IsConfirmed { get; set; }

    public User() { }
}