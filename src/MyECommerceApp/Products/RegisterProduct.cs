using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Annotations;
using FluentValidation;
using MassTransit;
using MyECommerceApp.Infrastructure.EntityFramework;
using System.Text.Json.Serialization;
using MyECommerceApp.Infrastructure.Host;

namespace MyECommerceApp.Products;

public class RegisterProduct : BaseFunction
{
    public class Command
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        [JsonIgnore]
        public bool Any { get; set; }
    }

    public class Result
    {
        public Guid ProductId { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(command => command.Name).MaximumLength(250).NotEmpty();
            RuleFor(command => command.Description).MaximumLength(500).NotEmpty();
            RuleFor(command => command.Price).GreaterThan(0);
        }
    }

    public class Handler
    {
        private readonly ApplicationDbContext _productRepository;

        public Handler(ApplicationDbContext productRepository)
        {
            _productRepository = productRepository;
        }

        public Task<Result> Handle(Command command)
        {
            var product = new Product(NewId.Next().ToSequentialGuid(), 
                command.Name, 
                command.Description, 
                command.Price,
                command.Any);

            _productRepository.Add(product);

            return Task.FromResult(new Result()
            {
                ProductId = product.ProductId
            });
        }
    }

    [LambdaFunction]
    [RestApi(LambdaHttpMethod.Post, "/products")]
    public Task<IHttpResult> Handle(
        [FromServices] AnyProducts.Runner runner,
        [FromServices] TransactionBehavior behavior,
        [FromServices] Handler handler,
        [FromBody] Command command)
    {
        return Handle(async () =>
        {
            new Validator().ValidateAndThrow(command);
            command.Any = await runner.Run(new AnyProducts.Query() { Name = command.Name });
            var result = await behavior.Handle(() => handler.Handle(command));
            return result;
        });
    }
}
