using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Annotations;
using MyECommerceApp.Infrastructure.EntityFramework;
using MyECommerceApp.Infrastructure.SqlKata;
using MyECommerceApp.Infrastructure.Host;

namespace MyECommerceApp.ClientRequests
{
    public class ListClientRequests : BaseFunction
    {
        public class Query : ListQuery
        {
            public string Name { get; set; }
        }

        public class Result
        {
            public Guid ClientRequestId { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public string PhoneNumber { get; set; }
            public string Status { get; set; }
            public DateTimeOffset RegisteredAt { get; set; }
            public DateTimeOffset? ApprovedAt { get; set; }
            public DateTimeOffset? RejectedAt { get; set; }
        }

        public class Runner : BaseRunner
        {
            public Runner(SqlKataQueryRunner queryRunner): base(queryRunner) { }

            public Task<ListResults<Result>> Run(Query query)
            {
                return _queryRunner.List<Query, Result>((qf)=> {
                    var statement = qf.Query(Tables.ClientRequests);

                    if (!string.IsNullOrEmpty(query.Name))
                    {
                        statement = statement.WhereLike(Tables.ClientRequests.Field(nameof(Query.Name)), $"%{query.Name}%");
                    }
                    return statement;
                }, query);
            }
        }

        [LambdaFunction]
        [RestApi(LambdaHttpMethod.Get, "/client-requests")]
        public Task<IHttpResult> Handle(
            [FromServices] Runner runner,
            [FromQuery] string name,
            [FromQuery] int page,
            [FromQuery] int pageSize)
        {
            return Handle(() => runner.Run(new Query() { Name = name, Page = page, PageSize = pageSize }));
        }
    }
}
