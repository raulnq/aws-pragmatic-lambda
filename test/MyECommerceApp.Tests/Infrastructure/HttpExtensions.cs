using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using MyECommerceApp.Infrastructure.ExceptionHandling;
using System.Web;

namespace MyECommerceApp.Tests.Infrastructure;

public static class HttpExtensions
{
    public static async Task<Result<TResponse>> Post<TRequest, TResponse>(this HttpClient client, string requestUri, TRequest request)
        where TResponse : class
    {
        var requestbody = JsonSerializer.Serialize(request);

        var httpResponse = await client.PostAsync(requestUri, new StringContent(requestbody, Encoding.Default, "application/json"));

        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

        options.Converters.Add(new JsonStringEnumConverter());

        var responseBody = await httpResponse.Content.ReadAsStringAsync();

        if (httpResponse.IsSuccessStatusCode)
        {
            var response = JsonSerializer.Deserialize<TResponse>(responseBody, options);

            return new Result<TResponse>(response);
        }
        else
        {
            var error = JsonSerializer.Deserialize<ProblemDetails>(responseBody, options);

            return new Result<TResponse>(error);
        }
    }

    public static async Task<Result> Post<TRequest>(this HttpClient client, string requestUri, TRequest request)
    {
        var requestbody = JsonSerializer.Serialize(request);

        var httpResponse = await client.PostAsync(requestUri, new StringContent(requestbody, Encoding.Default, "application/json"));

        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

        options.Converters.Add(new JsonStringEnumConverter());

        var responseBody = await httpResponse.Content.ReadAsStringAsync();

        if (httpResponse.IsSuccessStatusCode)
        {
            return new Result();
        }
        else
        {
            var error = JsonSerializer.Deserialize<ProblemDetails>(responseBody, options);

            return new Result(error);
        }
    }


    public static async Task<Result<TResponse>> Get<TResponse>(this HttpClient client, string requestUri)
        where TResponse : class
    {
        var httpResponse = await client.GetAsync(requestUri);

        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

        options.Converters.Add(new JsonStringEnumConverter());

        var responseBody = await httpResponse.Content.ReadAsStringAsync();

        if (httpResponse.IsSuccessStatusCode)
        {
            var response = JsonSerializer.Deserialize<TResponse>(responseBody, options);

            return new Result<TResponse>(response);
        }
        else
        {
            var error = JsonSerializer.Deserialize<ProblemDetails>(responseBody, options);

            return new Result<TResponse>(error);
        }
    }

    public static Task<Result<TResponse>> Get<TRequest, TResponse>(this HttpClient client, string requestUri, TRequest request)
        where TResponse : class
    {
        var uriBuilder = new UriBuilder($"host/{requestUri.TrimStart('/')}");

        var query = HttpUtility.ParseQueryString(uriBuilder.Query);

        foreach (var param in typeof(TRequest).GetProperties())
        {
            var value = param.GetValue(request)?.ToString();
            if (!string.IsNullOrEmpty(value))
            {
                query[param.Name.ToLower()] = value;
            }
        }

        uriBuilder.Query = query.ToString();

        return client.Get<TResponse>(uriBuilder.Uri.PathAndQuery);
    }
}
