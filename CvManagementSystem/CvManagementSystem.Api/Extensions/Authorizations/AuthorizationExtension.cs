using UserService.Api.Settings;
using UserService.Api.Utility;

namespace UserService.Api.Extensions.Authorizations;

public static class AuthorizationExtension
{
    public static void AddAuthorizationWithDefaultRoles(this IServiceCollection services, IConfiguration configuration)
    {
        var defaultPolicies = configuration.GetSection(nameof(DefaultRolePoliciesSettings)).Get<DefaultRolePoliciesSettings>();
        if (defaultPolicies == null)
        {
            throw new ArgumentNullException(nameof(defaultPolicies), "Default role policies are missing.");
        }
        
        services.AddAuthorizationBuilder()
                    .AddPolicy(defaultPolicies.AllowAdmin.Name, policy => policy.RequireRole(defaultPolicies.AllowAdmin.AllowedRoles))
                    .AddPolicy(defaultPolicies.AllowRecruiter.Name, policy => policy.RequireRole(defaultPolicies.AllowRecruiter.AllowedRoles))
                    .AddPolicy(defaultPolicies.AllowRegular.Name, policy => policy.RequireRole(defaultPolicies.AllowRegular.AllowedRoles));
        
        DefaultRolePolicies.Initialize(defaultPolicies);
    }
}