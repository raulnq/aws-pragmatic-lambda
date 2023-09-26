using MyECommerceApp.ClientRequests;
using Shouldly;
using Bogus;
using Xunit.Abstractions;
using MyECommerceApp.Tests.Infrastructure;

namespace MyECommerceApp.Tests.ClientRequests;


public class ClientRequestsDsl : Dsl
{
    private readonly HttpClient _httpClient;
    private readonly string _path = "Prod/client-requests";

    public ClientRequestsDsl(HttpClient httpClient, ITestOutputHelper output) : base(output)
    {
        _httpClient = httpClient;
    }

    public async Task<(RegisterClientRequest.Command, RegisterClientRequest.Result)> Register(Action<RegisterClientRequest.Command>? setup = null, string? error = null)
    {
        var faker = new Faker<RegisterClientRequest.Command>()
            .RuleFor(command => command.Name, faker => faker.Random.Guid().ToString())
            .RuleFor(command => command.Address, faker => faker.Address.FullAddress())
            .RuleFor(command => command.PhoneNumber, faker => faker.Phone.PhoneNumber("###-#####"));

        var request = faker.Generate();

        setup?.Invoke(request);

        var result = await _httpClient.Post<RegisterClientRequest.Command, RegisterClientRequest.Result>(_path, request);

        result.Check(error, successAssert: result =>
        {
            result.ClientRequestId.ShouldNotBe(Guid.Empty);
        });

        return (request, result.Response!);
    }

    public async Task<ApproveClientRequest.Command> Approve(Action<ApproveClientRequest.Command>? setup = null, string? error = null)
    {
        var request = new ApproveClientRequest.Command();

        setup?.Invoke(request);

        var result = await _httpClient.Post($"{_path}/{request.ClientRequestId}/approve", request);

        result.Check(error);

        return request;
    }    
}