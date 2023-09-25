using Microsoft.Extensions.DependencyInjection;

namespace MyECommerceApp.ShoppingCart.Host;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AdShoppingCart(this IServiceCollection services)
    {
        services.AddTransient<AddProductToShoppingCart.Handler>();

        services.AddTransient<AnyShoppingCartItems.Runner>();

        services.AddTransient<ListShoppingCartItems.Runner>();

        return services;
    }
}
