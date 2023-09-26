using MyECommerceApp.Infrastructure.EntityFramework;
using MyECommerceApp.Infrastructure.SqlKata;

namespace MyECommerceApp.ShoppingCart;

public class AnyShoppingCartItems
{
    public class Query
    {
        public Guid? ProductId { get; set; }
        public Guid? ClientId { get; set; }
    }

    public class Runner : BaseRunner
    {
        public Runner(SqlKataQueryRunner queryRunner) : base(queryRunner) { }

        public Task<bool> Run(Query query)
        {
            return _queryRunner.Any((qf) => {
                var statement = qf.Query(Tables.ShoppingCartItems);

                if (query.ProductId.HasValue)
                {
                    statement = statement.Where(Tables.ShoppingCartItems.Field(nameof(Query.ProductId)), query.ProductId.Value);
                }

                if (query.ClientId.HasValue)
                {
                    statement = statement.Where(Tables.ShoppingCartItems.Field(nameof(Query.ClientId)), query.ClientId.Value);
                }

                return statement;
            });
        }
    }
}
