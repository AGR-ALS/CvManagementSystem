using UserService.Api.Settings;

namespace UserService.Api.Utility;

public static class DefaultRolePolicies
{
    public static string AllowAdmin = null!;
    public static string AllowRecruiter = null!;
    public static string AllowRegular = null!;
    
    public static void Initialize(DefaultRolePoliciesSettings settings)
    {
        AllowAdmin = settings.AllowAdmin.Name;
        AllowRecruiter = settings.AllowRecruiter.Name;
        AllowRegular = settings.AllowRegular.Name;
    }
}