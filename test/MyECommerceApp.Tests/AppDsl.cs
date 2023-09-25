using MyECommerceApp.Tests.ClientRequests;
using MyECommerceApp.Tests.Orders;
using MyECommerceApp.Tests.Products;
using MyECommerceApp.Tests.ShoppingCart;
using Xunit.Abstractions;

namespace MyECommerceApp.Tests;

public class AppDsl : IAsyncDisposable
{
    private readonly HttpClient _client;

    public AppDsl(ITestOutputHelper output)
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri(ApiGatewayUrlProvider.Url)
        };

        ClientRequests = new ClientRequestsDsl(_client, output);

        Clients = new ClientsDsl(_client, output);

        Products = new ProductsDsl(_client, output);

        ShoppingCart = new ShoppingCartDsl(_client, output);

        Orders = new OrdersDsl(_client, output);
    }

    public OrdersDsl Orders { get; set; }

    public ClientRequestsDsl ClientRequests { get; }

    public ShoppingCartDsl ShoppingCart { get; set; }

    public ClientsDsl Clients { get; }

    public ProductsDsl Products { get; }

    public ValueTask DisposeAsync()
    {
        _client.Dispose();

        return ValueTask.CompletedTask;
    }
}
