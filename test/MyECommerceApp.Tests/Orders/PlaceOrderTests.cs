﻿using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace MyECommerceApp.Tests.Orders;

public class PlaceOrderTests : IAsyncLifetime
{
    private readonly AppDsl _appDsl;

    public PlaceOrderTests(ITestOutputHelper output)
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
    public async Task place_should_be_ok()
    {
        var (clientRequestCommand, clientRequestResult) = await _appDsl.ClientRequests.Register();

        await _appDsl.ClientRequests.Approve(c => c.ClientRequestId = clientRequestResult.ClientRequestId);

        await _appDsl.Clients.View(q => q.ClientId = clientRequestResult.ClientRequestId, successAssert: r => r.Name.ShouldBe(clientRequestCommand.Name));

        var (_, productResult) = await _appDsl.Products.Register();

        await _appDsl.Products.Enable(c => c.ProductId = productResult.ProductId);

        await _appDsl.ShoppingCart.AddProduct(c =>
        {
            c.ProductId = productResult.ProductId;
            c.ClientId = clientRequestResult.ClientRequestId;
        });

        await _appDsl.Orders.PlaceOrder(c => c.ClientId = clientRequestResult.ClientRequestId);
    }
}
