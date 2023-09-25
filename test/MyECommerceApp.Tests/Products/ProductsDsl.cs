using MyECommerceApp.Products;
using Shouldly;
using Bogus;
using Xunit.Abstractions;

namespace MyECommerceApp.Tests.Products;

public class ProductsDsl : Dsl
{
    private readonly HttpClient _httpClient;
    private readonly string _path = "Prod/products";

    public ProductsDsl(HttpClient httpClient, ITestOutputHelper output) : base(output)
    {
        _httpClient = httpClient;
    }

    public async Task<(RegisterProduct.Command, RegisterProduct.Result)> Register(Action<RegisterProduct.Command>? setup = null, string? error = null)
    {
        var faker = new Faker<RegisterProduct.Command>()
            .RuleFor(command => command.Name, faker => faker.Random.Guid().ToString())
            .RuleFor(command => command.Description, faker => faker.Lorem.Sentence())
            .RuleFor(command => command.Price, faker => faker.Random.Number(0,1000));

        var request = faker.Generate();

        setup?.Invoke(request);

        var result = await _httpClient.Post<RegisterProduct.Command, RegisterProduct.Result>(_path, request);

        result.Check(error, successAssert: result =>
        {
            result.ProductId.ShouldNotBe(Guid.Empty);
        });

        return (request, result.Response!);
    }

    public async Task<EnableProduct.Command> Enable(Action<EnableProduct.Command>? setup = null, string? error = null)
    {
        var request = new EnableProduct.Command();

        setup?.Invoke(request);

        var result = await _httpClient.Post($"{_path}/{request.ProductId}/enable", request);

        result.Check(error);

        return request;
    }
}