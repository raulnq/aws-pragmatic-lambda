using Shouldly;
using MyECommerceApp.Clients;
using Xunit.Abstractions;
using MyECommerceApp.Tests.Infrastructure;

namespace MyECommerceApp.Tests.ClientRequests;

public class ClientsDsl : Dsl
{
    private readonly HttpClient _httpClient;
    private readonly string _path = "Prod/clients";

    public ClientsDsl(HttpClient httpClient, ITestOutputHelper output) : base(output)
    {
        _httpClient = httpClient;
    }

    public async Task<(GetClients.Query, GetClients.Result)> View(Action<GetClients.Query>? setup = null, string? error = null, Action<GetClients.Result>? successAssert = null)
    {
        var request = new GetClients.Query();

        setup?.Invoke(request);

        if (successAssert == null)
        {
            var result = await _httpClient.Get<GetClients.Query, GetClients.Result>($"{_path}/{request.ClientId}", request);

            result.Check(error, successAssert: result =>
            {
                result.ClientId.ShouldNotBe(Guid.Empty);
            });

            return (request, result.Response!);
        }
        else
        {
           var result = new Result<GetClients.Result>();

           await WaitFor(async () =>
            {
                result = await _httpClient.Get<GetClients.Query, GetClients.Result>($"{_path}/{request.ClientId}", request);

                result.Check(error, successAssert: result =>
                {
                    successAssert(result);
                });

            }, TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(5));

            return (request, result.Response!);
        }
    }  
}