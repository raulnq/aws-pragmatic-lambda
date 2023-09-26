using Shouldly;
using Bogus;
using MyECommerceApp.ShoppingCart;
using Xunit.Abstractions;
using MyECommerceApp.Tests.Infrastructure;

namespace MyECommerceApp.Tests.ShoppingCart;

public class ShoppingCartDsl : Dsl
{
    private readonly HttpClient _httpClient;
    private readonly string _path = "Prod/shopping-cart";

    public ShoppingCartDsl(HttpClient httpClient, ITestOutputHelper output) : base(output)
    {
        _httpClient = httpClient;
    }

    public async Task<(AddProductToShoppingCart.Command, AddProductToShoppingCart.Result)> AddProductForClient(Action<AddProductToShoppingCart.Command>? setup = null, string? error = null)
    {
        var faker = new Faker<AddProductToShoppingCart.Command>()
            .RuleFor(command => command.Quantity, faker => faker.Random.Number(0,1000));

        var request = faker.Generate();

        setup?.Invoke(request);

        var result = await _httpClient.Post<AddProductToShoppingCart.Command, AddProductToShoppingCart.Result>(_path, request);

        result.Check(error, successAssert: result =>
        {
            result.ShoppingCartItemId.ShouldNotBe(Guid.Empty);
        });

        return (request, result.Response!);
    }
}