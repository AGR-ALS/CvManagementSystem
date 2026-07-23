using UserService.Api.Settings;

namespace UserService.Api.Extensions.Cors;

public static class CorsExtension
{
    public static string AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        var corsSettings = configuration.GetSection(nameof(CorsSettings)).Get<CorsSettings>();
        services.AddCors(options =>
        {
            options.AddPolicy(corsSettings!.PolicyName, policy =>
            {
                policy.WithOrigins(corsSettings.AllowedOrigin)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
        
        return corsSettings!.PolicyName;
    }
}