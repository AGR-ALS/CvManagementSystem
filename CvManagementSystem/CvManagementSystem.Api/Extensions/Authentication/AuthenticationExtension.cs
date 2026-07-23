using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using UserService.Api.Settings;
using CvManagementSystem.Infrastructure.Authentication.Jwt;
using CvManagementSystem.Infrastructure.Authentication.Tokens.Settings;

namespace UserService.Api.Extensions.Authentication;

public static class AuthenticationExtension
{
    public static void AddAuthenticationWithJwtScheme(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();
        var tokenIdentifiers = configuration.GetSection(nameof(TokenIdentifiers)).Get<TokenIdentifiers>();
        var oAuthAuthenticationSettings = configuration.GetSection(nameof(OAuthAuthenticationSettings)).Get<OAuthAuthenticationSettings>();

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.None;
            })
            .AddJwtBearer(
            JwtBearerDefaults.AuthenticationScheme,
            options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings!.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings!.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings!.SecretKey)),
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (tokenIdentifiers?.AccessTokenIdentifier != null)
                        {
                            context.Token = context.Request.Cookies[tokenIdentifiers.AccessTokenIdentifier];
                        }
                        
                        return Task.CompletedTask;
                    }
                };
            })
            .AddGoogle(nameof(oAuthAuthenticationSettings.Google), options =>
            {
                options.ClientId = oAuthAuthenticationSettings?.Google.ClientId!;
                options.ClientSecret = oAuthAuthenticationSettings?.Google.ClientSecret!;
    
                options.CallbackPath = oAuthAuthenticationSettings?.Google.CallbackPath!;
    
                options.SaveTokens = true; 
            })
            .AddFacebook(nameof(oAuthAuthenticationSettings.Facebook), options =>
            {
                options.ClientId = oAuthAuthenticationSettings?.Facebook.ClientId!;
                options.ClientSecret = oAuthAuthenticationSettings?.Facebook.ClientSecret!;
    
                options.CallbackPath = oAuthAuthenticationSettings?.Facebook.CallbackPath!;
    
                options.SaveTokens = true; 
            });
    }
}