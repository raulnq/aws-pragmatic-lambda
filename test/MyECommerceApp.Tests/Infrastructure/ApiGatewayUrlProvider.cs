using System.Diagnostics;

namespace MyECommerceApp.Tests.Infrastructure;

public class ApiGatewayUrlProvider
{
    private static readonly Lazy<string> _lazy = new Lazy<string>(() => Build());

    public readonly static string Url = _lazy.Value;

    private static string Build()
    {
        var startInfo = new ProcessStartInfo()
        {
            FileName = "aws",
            Arguments = "apigateway get-rest-apis --endpoint-url=http://localhost:4566 --query \"items[0].id\"",
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
        };

        var process = Process.Start(startInfo);

        if (process is null)
        {
            throw new ApplicationException("Missing Gateway URL");
        }

        var output = process.StandardOutput.ReadLine();

        if (string.IsNullOrEmpty(output))
        {
            throw new ApplicationException("Missing Gateway URL");
        }

        var apiGatewayId = output.Replace("\"", "");

        if (string.IsNullOrEmpty(apiGatewayId) || apiGatewayId.Contains("null"))
        {
            throw new ApplicationException("Missing Gateway URL");
        }

        return $"http://{apiGatewayId}.execute-api.localhost.localstack.cloud:4566";
    }
}
