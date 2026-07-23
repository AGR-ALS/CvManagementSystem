using Microsoft.AspNetCore.Authorization;
using UserService.Api.Settings;
using UserService.Api.Utility;

namespace UserService.Api.Attributes;

public class AllowRecruiterAttribute : AuthorizeAttribute
{
    public AllowRecruiterAttribute()
    {
        Policy = DefaultRolePolicies.AllowRecruiter;
    }
}