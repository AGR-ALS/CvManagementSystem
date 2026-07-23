namespace UserService.Api.Settings;

public class RolePolicy
{
    public string Name { get; set; } = null!;
    public string[] AllowedRoles { get; set; } = null!;
}