using MyECommerceApp.Infrastructure.EntityFramework;
using MyECommerceApp.Infrastructure.SqlKata;

namespace MyECommerceApp.ShoppingCart;

public class ListShoppingCartItems
{
    public class Query
    {
        public Guid ClientId { get; set; }
    }

    public class Result
    {
        public Guid ClientId { get; set; }
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
    }

    public class Runner : BaseRunner
    {
        public Runner(SqlKataQueryRunner queryRunner): base(queryRunner) { }

        public Task<List<Result>> Run(Query query)
        {
            return _queryRunner.List<Result>((qf)=> {
                var statement = qf.Query(Tables.ShoppingCartItems)
                    .Join(Tables.Products, Tables.Products.Field("ProductId"), Tables.ShoppingCartItems.Field("ProductId"))
                    .Where(Tables.ShoppingCartItems.Field(nameof(Query.ClientId)), query.ClientId);
                return statement;
            });
        }
    }
}
