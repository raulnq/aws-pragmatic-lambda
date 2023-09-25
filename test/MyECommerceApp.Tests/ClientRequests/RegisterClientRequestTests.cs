using Xunit;
using Xunit.Abstractions;

namespace MyECommerceApp.Tests.ClientRequests;

public class RegisterClientRequestTests : IAsyncLifetime
{
    private readonly AppDsl _appDsl;

    public RegisterClientRequestTests(ITestOutputHelper output)
    {
        _appDsl = new AppDsl(output);
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        return _appDsl.DisposeAsync().AsTask();
    }


    [Fact]
    public Task register_should_be_ok()
    {
        return _appDsl.ClientRequests.Register();
    }
}
