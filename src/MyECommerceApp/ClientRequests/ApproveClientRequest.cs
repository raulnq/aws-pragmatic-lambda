using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Annotations;
using MyECommerceApp.Infrastructure.EntityFramework;
using MyECommerceApp.Infrastructure.Messaging;
using MyECommerceApp.Infrastructure.Host;
using AWS.Lambda.Powertools.Logging;
using AWS.Lambda.Powertools.Tracing;

namespace MyECommerceApp.ClientRequests;

public class ApproveClientRequest: BaseFunction
{
    public class Command
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

        public async Task Handle(Command command)
        {
            var clientRequest = await _context.Get<ClientRequest>(command.ClientRequestId);

            clientRequest.Approve();
        }
    }

    [LambdaFunction]
    [Logging]
    [Tracing]
    [RestApi(LambdaHttpMethod.Post, "/client-requests/{clientRequestId}/approve")]
    public Task<IHttpResult> Handle(
        [FromServices] TransactionBehavior behavior,
        [FromServices] Handler handler,
        [FromServices] EventPublisher publisher,
        string clientRequestId)
    {
        return Handle(async () =>
        {
            var command = new Command() { ClientRequestId = Guid.Parse(clientRequestId) };
            await behavior.Handle(() => handler.Handle(command));
            await publisher.Publish(new ClientRequestApproved(command.ClientRequestId));
        });
    }
}
