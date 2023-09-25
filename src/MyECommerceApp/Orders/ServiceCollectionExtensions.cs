using Microsoft.Extensions.DependencyInjection;

namespace MyECommerceApp.Orders;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrders(this IServiceCollection services)
    {
        services.AddTransient<PlaceOrder.Handler>();

        return services;
    }
}
