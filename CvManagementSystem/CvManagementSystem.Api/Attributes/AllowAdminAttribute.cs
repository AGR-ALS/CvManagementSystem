using Microsoft.AspNetCore.Authorization;
using UserService.Api.Settings;
using UserService.Api.Utility;

namespace UserService.Api.Attributes;

public class AllowAdminAttribute : AuthorizeAttribute
{
    public AllowAdminAttribute()
    {
        Policy = DefaultRolePolicies.AllowAdmin;
    }
}