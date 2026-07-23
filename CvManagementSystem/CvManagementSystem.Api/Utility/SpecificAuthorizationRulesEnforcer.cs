using System.Security.Claims;
using Microsoft.Extensions.Options;
using UserService.Api.Exceptions;
using UserService.Api.Settings;
using UserService.Application.Abstractions.Utility;

namespace UserService.Api.Utility;

public class SpecificAuthorizationRulesEnforcer : ISpecificAccessRulesEnforcer
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly DefaultRolePoliciesSettings _defaultRolePoliciesSettings;

    public SpecificAuthorizationRulesEnforcer(IOptions<DefaultRolePoliciesSettings> defaultRolePoliciesSettings, IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _defaultRolePoliciesSettings = defaultRolePoliciesSettings.Value;
    }
    
    public void CheckIfRegularOwnsDataOrHasHigherRole(Guid ownerId)
    {
        if (_httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value != ownerId.ToString() && 
            !_defaultRolePoliciesSettings.AllowRecruiter.AllowedRoles.Contains(_httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value))
        {
            throw new ForbidException("You do not have access to this data");
        }
    }
    
    public void CheckIfRegularOwnsDataOrHasHighestRole(Guid ownerId)
    {
        if (_httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value != ownerId.ToString() && 
            !_defaultRolePoliciesSettings.AllowAdmin.AllowedRoles.Contains(_httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value))
        {
            throw new ForbidException("You do not have access to this data");
        }
    }
}