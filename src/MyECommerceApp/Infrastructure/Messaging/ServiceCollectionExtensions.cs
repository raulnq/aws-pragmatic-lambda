using Amazon.Extensions.NETCore.Setup;
using Amazon.SimpleNotificationService;
using Humanizer.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MyECommerceApp.Infrastructure.Messaging;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        var isLocal = configuration.GetValue("IsLocal", false);
        if(isLocal)
        {
            var awsOptions = configuration.GetAWSOptions();
            awsOptions.DefaultClientConfig.ServiceURL = "http://localhost:4566";
            awsOptions.DefaultClientConfig.UseHttp = true;
            services.AddDefaultAWSOptions(awsOptions);
        }

        services.AddAWSService<IAmazonSimpleNotificationService>();

        services.AddScoped(f =>
        {
            var client = f.GetRequiredService<IAmazonSimpleNotificationService>();

            return new EventPublisher(client, configuration["TopicArn"]);
        });

        return services;
    }
}
