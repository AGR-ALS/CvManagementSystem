using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using UserService.Domain.Abstractions;
using CvManagementSystem.Infrastructure.Authentication.Tokens.Settings;

namespace UserService.Api.Middleware;

public class RefreshTokenAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly TokenIdentifiers _tokenIdentifiers;

    public RefreshTokenAuthenticationMiddleware(RequestDelegate next, IOptions<TokenIdentifiers> tokenIdentifiers)
    {
        _next = next;
        _tokenIdentifiers = tokenIdentifiers.Value;
    }

    public async Task InvokeAsync(HttpContext context, IUsersService usersService)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            await _next(context);
            
            return;
        }
        
        var refreshToken = context.Request.Cookies[_tokenIdentifiers.RefreshTokenIdentifier];
        await AuthenticateViaRefreshToken(context, usersService, refreshToken);
        
        await _next(context);
    }

    private async Task AuthenticateViaRefreshToken(HttpContext context, IUsersService usersService, string? refreshToken)
    {
        if (!string.IsNullOrEmpty(refreshToken))
        {
            try
            {
                await GetClaimsViaRefreshToken(context, usersService, refreshToken);
            }
            catch (UnauthorizedAccessException) 
            {
                context.Response.Cookies.Delete(_tokenIdentifiers.AccessTokenIdentifier);
                context.Response.Cookies.Delete(_tokenIdentifiers.RefreshTokenIdentifier);
            }
        }
    }

    private async Task GetClaimsViaRefreshToken(HttpContext context, IUsersService usersService, string refreshToken)
    {
        var newAccessToken = await usersService.LoginAsync(refreshToken, context.RequestAborted);
        context.Response.Cookies.Append(_tokenIdentifiers.AccessTokenIdentifier, newAccessToken);
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(newAccessToken);
        var claimsIdentity = new ClaimsIdentity(jwtToken.Claims, _tokenIdentifiers.AccessTokenIdentifier);
        context.User = new ClaimsPrincipal(claimsIdentity);
    }
}