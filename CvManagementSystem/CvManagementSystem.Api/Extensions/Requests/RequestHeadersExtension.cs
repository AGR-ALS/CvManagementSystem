using Microsoft.AspNetCore.HttpOverrides;

namespace UserService.Api.Extensions.Requests;

public static class RequestHeadersExtension
{
    public static void ConfigureRequestHeaders(this IServiceCollection services)
    {
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });
    }
}