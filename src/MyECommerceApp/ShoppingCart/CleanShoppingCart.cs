using Amazon.Lambda.Annotations;
using MyECommerceApp.Infrastructure.EntityFramework;
using Amazon.Lambda.SQSEvents;
using Microsoft.EntityFrameworkCore;
using MyECommerceApp.Orders;
using MyECommerceApp.Infrastructure.Host;

namespace MyECommerceApp.ShoppingCart;

public class CleanShoppingCartFunction : BaseFunction
{
    [LambdaFunction]
    public Task<SQSBatchResponse> Handle(
    [FromServices] TransactionBehavior behavior,
    [FromServices] ApplicationDbContext context,
    SQSEvent sqsEvent)
    {
        return HandleFromSubscription<OrderRegistered>(async (orderRegistered) =>
        {
            await behavior.Handle(() => Delete(context, orderRegistered.ClientId));
        }, sqsEvent);
    }

    public Task Delete(ApplicationDbContext context, Guid clientId)
    {
        var statement = "DELETE FROM dbo.ShoppingCartItems WHERE ClientId={0}";
        return context.Database.ExecuteSqlRawAsync(statement, clientId);
    }
}

