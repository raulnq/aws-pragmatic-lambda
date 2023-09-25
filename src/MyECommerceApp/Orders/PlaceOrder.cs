using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Annotations;
using FluentValidation;
using MassTransit;
using MyECommerceApp.Clients;
using MyECommerceApp.Infrastructure.EntityFramework;
using MyECommerceApp.Infrastructure.Messaging;
using MyECommerceApp.ShoppingCart;
using System.Text.Json.Serialization;
using MyECommerceApp.Infrastructure.Host;

namespace MyECommerceApp.Orders
{
    public static class PlaceOrder
    {
        public class Command
        {
            public Guid ClientId { get; set; }
            public PaymentMethod PaymentMethod { get; set; }
            public DateTimeOffset DeliveryDate { get; set; }
            [JsonIgnore]
            public List<ListShoppingCartItems.Result> ShoppingCartItems { get; set; }
            [JsonIgnore]
            public GetClients.Result Client { get; set; }
        }

        public class Result
        {
            public Guid OrderId { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(command => command.DeliveryDate).GreaterThan(DateTimeOffset.UtcNow).NotEmpty();
            }
        }

        public class Handler
        {
            private readonly ApplicationDbContext _contex;

            public Handler(ApplicationDbContext orderRepository)
            {
                _contex = orderRepository;
            }

            public Task<Result> Handle(Command command)
            {
                var order = new Order(NewId.Next().ToSequentialGuid(),
                    command.PaymentMethod, 
                    command.DeliveryDate, 
                    command.ClientId, 
                    command.Client.Address, 
                    command.ShoppingCartItems.Select(sci=>(sci.ProductId, sci.Name, sci.Price, sci.Quantity)).ToArray());

                _contex.Add(order);

                return Task.FromResult(new Result()
                {
                    OrderId = order.OrderId
                });
            }
        }
    }

    public class PlaceOrderFunction : BaseFunction
    {
        [LambdaFunction]
        [RestApi(LambdaHttpMethod.Post, "/orders")]
        public Task<IHttpResult> Handle(
        [FromServices] GetClients.Runner getClientRunner,
        [FromServices] ListShoppingCartItems.Runner listShoppingCartItemsRunner,
        [FromServices] TransactionBehavior behavior,
        [FromServices] PlaceOrder.Handler handler,
        [FromServices] EventPublisher publisher,
        [FromBody] PlaceOrder.Command command)
        {
            return Handle(async () =>
            {
                new PlaceOrder.Validator().ValidateAndThrow(command);
                command.Client = await getClientRunner.Run(new GetClients.Query() { ClientId = command.ClientId });
                command.ShoppingCartItems = await listShoppingCartItemsRunner.Run(new ListShoppingCartItems.Query() { ClientId = command.ClientId });
                var result = await behavior.Handle(() => handler.Handle(command));
                await publisher.Publish(new OrderRegistered(result.OrderId, command.ClientId));
                return result;
            });
        }
    }
}
