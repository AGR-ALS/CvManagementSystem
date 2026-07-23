namespace UserService.Api.Settings;

public class DefaultRolePoliciesSettings
{
    public RolePolicy AllowAdmin { get; set; } = null!;
    public RolePolicy AllowRecruiter { get; set; } = null!;
    public RolePolicy AllowRegular { get; set; } = null!;
}