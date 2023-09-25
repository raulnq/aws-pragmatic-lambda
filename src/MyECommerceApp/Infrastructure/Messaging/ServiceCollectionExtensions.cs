using Amazon.SimpleNotificationService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MyECommerceApp.Infrastructure.Messaging;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAWSService<IAmazonSimpleNotificationService>();

        services.AddScoped(f =>
        {
            var client = f.GetRequiredService<IAmazonSimpleNotificationService>();

            return new EventPublisher(client, configuration["TopicArn"]);
        });

        return services;
    }
}
