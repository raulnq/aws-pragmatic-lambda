using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Annotations;
using FluentValidation;
using MassTransit;
using MyECommerceApp.Infrastructure.EntityFramework;
using System.Text.Json.Serialization;
using MyECommerceApp.Infrastructure.Host;

namespace MyECommerceApp.ShoppingCart;

public class AddProductToShoppingCart : BaseFunction
{
    public class Command
    {
        public Guid ClientId { get; set; }
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
        [JsonIgnore]
        public bool Any { get; set; }
    }

    public class Result
    {
        public Guid ShoppingCartItemId { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(command => command.Quantity).GreaterThan(0);
        }
    }

    public class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<Result> Handle(Command command)
        {
            var product = new ShoppingCartItem(NewId.Next().ToSequentialGuid(), 
                command.ClientId, 
                command.ProductId, 
                command.Quantity,
                command.Any);

            _context.Add(product);

            return Task.FromResult(new Result()
            {
                ShoppingCartItemId = product.ShoppingCartItemId
            });
        }
    }

    [LambdaFunction]
    [RestApi(LambdaHttpMethod.Post, "/shopping-cart")]
    public Task<IHttpResult> Handle(
        [FromServices] AnyShoppingCartItems.Runner runner,
        [FromServices] TransactionBehavior behavior,
        [FromServices] Handler handler,
        [FromBody] Command command)
    {
        return Handle(async () =>
        {
            new Validator().ValidateAndThrow(command);
            command.Any = await runner.Run(new AnyShoppingCartItems.Query() { ClientId = command.ClientId, ProductId = command.ProductId });
            var result = await behavior.Handle(() => handler.Handle(command));
            return result;
        });
    }
}
