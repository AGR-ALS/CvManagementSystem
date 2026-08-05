using UserService.Api.Settings;
using UserService.Application.Settings;
using CvManagementSystem.Infrastructure.Authentication.AccountConfirmation;
using CvManagementSystem.Infrastructure.Authentication.Jwt;
using CvManagementSystem.Infrastructure.Authentication.RefreshTokens;
using CvManagementSystem.Infrastructure.Authentication.Tokens.Settings;
using CvManagementSystem.Infrastructure.Files;
using CvManagementSystem.Infrastructure.Integrations.Settings;
using CvManagementSystem.Infrastructure.Mail.Content;
using CvManagementSystem.Infrastructure.MessageEvents.Settings;
using UserService.Domain.Models;

namespace UserService.Api.Extensions.Services;

public static class OptionsConfiguringExtension
{
    public static void ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
        services.Configure<RefreshTokenSettings>(configuration.GetSection(nameof(RefreshTokenSettings)));
        services.Configure<TokenIdentifiers>(configuration.GetSection(nameof(TokenIdentifiers)));
        services.Configure<CorsSettings>(configuration.GetSection(nameof(CorsSettings)));
        services.Configure<RolesSettings>(configuration.GetSection(nameof(RolesSettings)));
        services.Configure<DefaultRolePoliciesSettings>(configuration.GetSection(nameof(DefaultRolePoliciesSettings)));
        services.Configure<FileUploadingSettings>(configuration.GetSection(nameof(FileUploadingSettings)));
        services.Configure<OAuthAuthenticationSettings>(configuration.GetSection(nameof(OAuthAuthenticationSettings)));
        services.Configure<AccountConfirmationEmailContent>(configuration.GetSection(nameof(AccountConfirmationEmailContent)));
        services.Configure<RabbitMqSettings>(configuration.GetSection(nameof(RabbitMqSettings)));
        services.Configure<AccountConfirmationTokenSettings>(configuration.GetSection(nameof(AccountConfirmationTokenSettings)));
        services.Configure<AccountConfirmationSettings>(configuration.GetSection(nameof(AccountConfirmationSettings)));
        services.Configure<S3StorageSettings>(configuration.GetSection(nameof(S3StorageSettings)));
        services.Configure<SalesforceSettings>(configuration.GetSection(nameof(SalesforceSettings)));
        services.Configure<PositionApiTokenSettings>(configuration.GetSection(nameof(PositionApiTokenSettings)));
        services.Configure<DropBoxSettings>(configuration.GetSection(nameof(DropBoxSettings)));
        services.Configure<SupportTicketSettings>(configuration.GetSection(nameof(SupportTicketSettings)));
    }
}