using Xunit;
using Xunit.Abstractions;

namespace MyECommerceApp.Tests.Products;

public class EnableProductTests : IAsyncLifetime
{
    private readonly AppDsl _appDsl;

    public EnableProductTests(ITestOutputHelper output)
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
    public async Task enable_should_be_ok()
    {
        var (_, result) = await _appDsl.Products.Register();

        await _appDsl.Products.Enable(c => c.ProductId = result.ProductId);
    }
}