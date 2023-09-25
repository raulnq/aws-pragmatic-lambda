using Microsoft.Extensions.DependencyInjection;

namespace MyECommerceApp.Clients;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddClients(this IServiceCollection services)
    {
        services.AddTransient<RegisterClient.Handler>();

        services.AddTransient<GetClients.Runner>();

        return services;
    }
}
