using Shouldly;
using Bogus;
using Xunit.Abstractions;
using MyECommerceApp.Orders;

namespace MyECommerceApp.Tests.Orders;

public class OrdersDsl : Dsl
{
    private readonly HttpClient _httpClient;
    private readonly string _path = "Prod/orders";

    public OrdersDsl(HttpClient httpClient, ITestOutputHelper output) : base(output)
    {
        _httpClient = httpClient;
    }

    public async Task<(PlaceOrder.Command, PlaceOrder.Result)> PlaceOrderForClient(Action<PlaceOrder.Command>? setup = null, string? error = null)
    {
        var faker = new Faker<PlaceOrder.Command>()
            .RuleFor(command => command.DeliveryDate, faker => DateTimeOffset.UtcNow.AddDays(1))
            .RuleFor(command => command.PaymentMethod, faker => PaymentMethod.Cash)
            ;

        var request = faker.Generate();

        setup?.Invoke(request);

        var result = await _httpClient.Post<PlaceOrder.Command, PlaceOrder.Result>(_path, request);

        result.Check(error, successAssert: result =>
        {
            result.OrderId.ShouldNotBe(Guid.Empty);
        });

        return (request, result.Response!);
    }
}