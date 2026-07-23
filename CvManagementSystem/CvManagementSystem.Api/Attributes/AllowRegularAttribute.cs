    using Microsoft.AspNetCore.Authorization;
using UserService.Api.Settings;
using UserService.Api.Utility;

namespace UserService.Api.Attributes;

public class AllowRegularAttribute : AuthorizeAttribute
{
    public AllowRegularAttribute()
    {
        Policy = DefaultRolePolicies.AllowRegular;
    }
}