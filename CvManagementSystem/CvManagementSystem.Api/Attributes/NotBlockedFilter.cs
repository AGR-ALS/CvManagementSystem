using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UserService.Domain.Abstractions;

namespace UserService.Api.Attributes;

public class NotBlockedFilter : IAsyncAuthorizationFilter
{
    private readonly IUsersService _usersService;

    public NotBlockedFilter(IUsersService usersService)
    {
        _usersService = usersService;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var id = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(id))
        {
            return;
        }
        
        if ((await _usersService.GetUserByIdAsync(new Guid(id))).IsBlocked)
        {
            context.Result = new ForbidResult();
        }
    }
}