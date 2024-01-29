using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Annotations;
using FluentValidation;
using MassTransit;
using MyECommerceApp.Infrastructure.EntityFramework;
using System.Text.Json.Serialization;
using MyECommerceApp.Infrastructure.Host;
using AWS.Lambda.Powertools.Logging;
using AWS.Lambda.Powertools.Tracing;

namespace MyECommerceApp.ClientRequests;

public class RegisterClientRequest : BaseFunction
{
    public class Command
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        [JsonIgnore]
        public bool Any { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(command => command.Name).MaximumLength(100).NotEmpty();
            RuleFor(command => command.Address).MaximumLength(500).NotEmpty();
            RuleFor(command => command.PhoneNumber).MaximumLength(20).NotEmpty();
        }
    }

    public class Result
    {
        public Guid ClientRequestId { get; set; }
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
            var clientRequest = new ClientRequest(
                NewId.Next().ToSequentialGuid(),
                command.Name,
                command.Address,
                command.PhoneNumber,
                command.Any);

            _context.Set<ClientRequest>().Add(clientRequest);

            return Task.FromResult(new Result()
            {
                ClientRequestId = clientRequest.ClientRequestId
            });
        }
    }

    [LambdaFunction]
    [RestApi(LambdaHttpMethod.Post, "/client-requests")]
    [Logging]
    [Tracing]
    public Task<IHttpResult> Handle(
        [FromServices] AnyClientRequests.Runner runner,
        [FromServices] TransactionBehavior behavior,
        [FromServices] Handler handler,
        [FromBody] Command command)
    {
        return Handle(async () =>
        {
            new Validator().ValidateAndThrow(command);
            command.Any = await runner.Run(new AnyClientRequests.Query() { Name = command.Name });
            var result = await behavior.Handle(() => handler.Handle(command));
            return result;
        });
    }
}
