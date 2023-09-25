using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace MyECommerceApp.Tests.ClientRequests;

public class ApproveClientRequestTests : IAsyncLifetime
{
    private readonly AppDsl _appDsl;

    public ApproveClientRequestTests(ITestOutputHelper  output)
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
    public async Task approve_should_be_ok()
    {
        var (command, result) = await _appDsl.ClientRequests.Register();

        await _appDsl.ClientRequests.Approve(c => c.ClientRequestId = result.ClientRequestId);

        await _appDsl.Clients.View(q => q.ClientId = result.ClientRequestId, successAssert: r => r.Name.ShouldBe(command.Name));
    }
}
