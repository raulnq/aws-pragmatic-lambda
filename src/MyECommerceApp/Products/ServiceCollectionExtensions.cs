using Microsoft.Extensions.DependencyInjection;

namespace MyECommerceApp.Products;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AdProducts(this IServiceCollection services)
    {
        services.AddTransient<RegisterProduct.Handler>();

        services.AddTransient<AnyProducts.Runner>();

        services.AddTransient<EnableProduct.Handler>();

        return services;
    }
}
