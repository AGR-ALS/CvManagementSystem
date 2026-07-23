using UserService.Domain.Models;

namespace UserService.Application.Settings;

public class RolesSettings
{
    public Role[] Roles { get; set; } = null!;
    public Guid DefaultRoleId { get; set; }
    public Guid RecruiterRoleId { get; set; }
}