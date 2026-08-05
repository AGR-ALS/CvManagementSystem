using Microsoft.AspNetCore.Identity;
using UserService.Api.Utility;
using UserService.Application.Abstractions.Authentication;
using UserService.Application.Abstractions.Authentication.Jwt;
using UserService.Application.Abstractions.Authentication.Services;
using UserService.Application.Abstractions.Mail;
using UserService.Application.Abstractions.MessageEvents;
using UserService.Application.Abstractions.Repositories;
using UserService.Application.Abstractions.Sevices;
using UserService.Application.Abstractions.Utility;
using UserService.Application.Services;
using UserService.Application.Utility;
using UserService.DataAccess.Repositories;
using UserService.DataAccess.Repositories.Factories;
using UserService.Domain.Abstractions;
using UserService.Domain.Models.Attributes;
using CvManagementSystem.Infrastructure.Authentication;
using CvManagementSystem.Infrastructure.Authentication.AccountConfirmation;
using CvManagementSystem.Infrastructure.Authentication.Jwt;
using CvManagementSystem.Infrastructure.Authentication.RefreshTokens;
using CvManagementSystem.Infrastructure.Files;
using CvManagementSystem.Infrastructure.Integrations;
using CvManagementSystem.Infrastructure.Integrations.Services;
using CvManagementSystem.Infrastructure.Mail;
using CvManagementSystem.Infrastructure.Mail.Abstractions;
using CvManagementSystem.Infrastructure.MessageEvents.Publishers;
using UserService.Application.Abstractions.Integrations;
using UserService.Application.Abstractions.Integrations.Services;

namespace UserService.Api.Extensions.Services;

public static class DiServicesExtension
{
    public static void AddDiServices(this IServiceCollection services)
    {
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IRolesRepository, RolesRepository>();
        services.AddScoped<IProjectsRepository, ProjectsRepository>();
        services.AddScoped<IDiscussionRepository, DiscussionRepository>();
        services.AddScoped<IRefreshTokensRepository, RefreshTokensRepository>();
        services.AddScoped(typeof(IAttributeValuesRepository<>), typeof(AttributeValuesRepository<>));
        services.AddScoped<IAttributeCategoriesRepository, AttributeCategoriesRepository>();
        services.AddScoped<IAttributeDefinitionsRepository, AttributeDefinitionsRepository>();
        services.AddScoped<IAttributeValuesRepositoryFactory, AttributeValuesRepositoryFactory>();
        services.AddScoped<IPositionsRepository, PositionsRepository>();
        services.AddScoped<ICvsRepository, CvsRepository>();
        services.AddScoped<IPositionsService, PositionsService>();
        services.AddScoped<ICvsService, CvsService>();
        services.AddScoped<ITechnologiesRepository, TechnologiesRepository>();
        services.AddScoped<ITechnologiesService, TechnologiesService>();
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<IProjectsService, ProjectsService>();
        services.AddScoped<IDiscussionService, DiscussionService>();
        services.AddScoped<IAttributesService, AttributesService>();
        services.AddScoped<IRefreshTokensService, RefreshTokensService>();
        services.AddScoped<IFileStorageService, S3FileStorageService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<ISecureTokenGenerator, SecureTokenGenerator>();
        services.AddScoped<ISpecificAccessRulesEnforcer, SpecificAuthorizationRulesEnforcer>();
        services.AddScoped<IOptionsRepository, OptionsRepository>();
        services.AddScoped<IAccessRuleEnforcer, AccessRuleEnforcer>();
        services.AddScoped<IAccountConfirmationTokensRepository, AccountConfirmationTokensRepository>();
        services.AddScoped<IAccountConfirmationTokensRepository, AccountConfirmationTokensRepository>();
        services.AddScoped<IAccountConfirmationTokensService, AccountConfirmationTokensService>();
        services.AddScoped<IMailService, MailService>();
        services.AddScoped<IMailEventPublisher, MailEventPublisher>();
        services.AddScoped<IEmailBuilder, EmailBuilder>();
        services.AddScoped<ILinkBuilder, EmailBuilder>();
        services.AddScoped<ISalesforceService, SalesforceService>();
        services.AddScoped<ISalesforceRecordsRepository, SalesforceRecordsRepository>();
        services.AddScoped<IPositionApiTokensRepository, PositionApiTokensRepository>();
        services.AddScoped<IPositionApiTokensService, PositionApiTokensService>();
        services.AddScoped<ISupportTicketService, SupportTicketService>();
        services.AddScoped<IPositionImportRepository, PositionsRepository>();
        services.AddScoped<IOdooService, OdooService>();
    }
}