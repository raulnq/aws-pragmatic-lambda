using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace MyECommerceApp.Infrastructure.SqlKata;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSqlKata(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<QueryFactory>(_ =>
        {
            var connection = new SqlConnection(configuration["DbConnectionString"]);

            var compiler = new SqlServerCompiler() { UseLegacyPagination = false };

            var factory = new QueryFactory(connection, compiler);

            return factory;
        });

        services.AddScoped<SqlKataQueryRunner>();

        return services;
    }
}
