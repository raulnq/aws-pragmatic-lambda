using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Annotations;
using MyECommerceApp.Infrastructure.EntityFramework;
using MyECommerceApp.Infrastructure.SqlKata;
using MyECommerceApp.Infrastructure.Host;

namespace MyECommerceApp.Clients
{
    public class GetClients
    {
        public class Query
        {
            public Guid ClientId { get; set; }
        }

        public class Result
        {
            public Guid ClientId { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public string PhoneNumber { get; set; }
        }

        public class Runner : BaseRunner
        {
            public Runner(SqlKataQueryRunner queryRunner) : base(queryRunner) { }

            public Task<Result> Run(Query query)
            {
                return _queryRunner.Get<Result>((qf) => qf
                    .Query(Tables.Clients)
                    .Where(Tables.Clients.Field(nameof(Query.ClientId)), query.ClientId));
            }
        }
    }

    public class GetClientsFunction : BaseFunction
    {
        [LambdaFunction]
        [RestApi(LambdaHttpMethod.Get, "/clients/{clientId}")]
        public Task<IHttpResult> Handle(
        [FromServices] GetClients.Runner runner,
        string clientId)
        {
            return Handle(() => runner.Run(new GetClients.Query() { ClientId = Guid.Parse(clientId) }));
        }
    }

}
