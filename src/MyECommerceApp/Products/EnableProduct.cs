using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Annotations;
using MyECommerceApp.Infrastructure.EntityFramework;
using MyECommerceApp.Infrastructure.Host;

namespace MyECommerceApp.Products;

public class EnableProduct: BaseFunction
{
    public class Command
    {
        public Guid ProductId { get; set; }
    }

    public class Handler
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Handle(Command command)
        {
            var clientRequest = await _context.Get<Product>(command.ProductId);

            clientRequest.Enable();
        }
    }

    [LambdaFunction]
    [RestApi(LambdaHttpMethod.Post, "/products/{productId}/enable")]
    public Task<IHttpResult> Handle(
        [FromServices] TransactionBehavior behavior,
        [FromServices] Handler handler,
    string productId)
    {
        return Handle(async () =>
        {
            var command = new Command() { ProductId = Guid.Parse(productId) };
            await behavior.Handle(() => handler.Handle(command));
        });
    }
}
