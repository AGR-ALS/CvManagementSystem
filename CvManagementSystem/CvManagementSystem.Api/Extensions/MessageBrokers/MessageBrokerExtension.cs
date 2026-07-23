using MassTransit;
using Microsoft.Extensions.Options;
using CvManagementSystem.Infrastructure.MessageEvents.Settings;

namespace UserService.Api.Extensions.MessageBrokers;

public static class MessageBrokerExtension
{
    public static void AddRabbitMqViaMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqSettings>(configuration.GetSection(nameof(RabbitMqSettings)));
    
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            busConfigurator.UsingRabbitMq((context, configurator) =>
            {
                var brokerSettings = context.GetRequiredService<IOptions<RabbitMqSettings>>().Value;
    
                configurator.Host(new Uri(brokerSettings.Host), h =>
                {
                    h.Username(brokerSettings.Username);
                    h.Password(brokerSettings.Password);
                });
                configurator.ConfigureEndpoints(context);
            });
        });
    }
}