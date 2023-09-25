using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.SQSEvents;
using AWS.Lambda.Powertools.Logging;
using FluentValidation;
using MyECommerceApp.Infrastructure.ExceptionHandling;
using System.Net;
using System.Text.Json;

namespace MyECommerceApp.Infrastructure.Host;

public class BaseFunction
{
    public async Task<IHttpResult> Handle<T>(Func<Task<T>> action)
    {
        try
        {
            var result = await action();

            return HttpResults.Ok(result);
        }
        catch (ValidationException vex)
        {
            return HttpResults.NotFound(new ProblemDetails()
            {
                Type = "validation-error",
                Detail = vex.Message,
                Status = (int)HttpStatusCode.BadRequest,
                Title = "Validation Error",
                Errors = vex.Errors.Where(failure => failure != null)
                .ToLookup(failure => failure.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(failure => failure.ErrorCode).ToArray())
            });
        }
        catch (NotFoundException ntex)
        {
            return HttpResults.NotFound(new ProblemDetails()
            {
                Type = "resource-not-found",
                Detail = ntex.Message,
                Status = (int)HttpStatusCode.NotFound,
                Title = "Resource Not Found"
            });
        }
        catch (DomainException dex)
        {
            return HttpResults.BadRequest(new ProblemDetails()
            {
                Type = "domain-error",
                Detail = dex.Message,
                Status = (int)HttpStatusCode.BadRequest,
                Title = "Domain Error"
            });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unexpected error {error}", ex.Message);

            return HttpResults.InternalServerError(new ProblemDetails()
            {
                Type = "internal-server-error",
                Detail = ex.StackTrace,
                Status = (int)HttpStatusCode.InternalServerError,
                Title = ex.Message
            });
        }
    }

    public async Task<IHttpResult> Handle(Func<Task> action)
    {
        try
        {
            await action();

            return HttpResults.Ok();
        }
        catch (ValidationException vex)
        {
            return HttpResults.NotFound(new ProblemDetails()
            {
                Type = "validation-error",
                Detail = vex.Message,
                Status = (int)HttpStatusCode.BadRequest,
                Title = "Validation Error",
                Errors = vex.Errors.Where(failure => failure != null)
                .ToLookup(failure => failure.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(failure => failure.ErrorCode).ToArray())
            });
        }
        catch (NotFoundException ntex)
        {
            return HttpResults.NotFound(new ProblemDetails()
            {
                Type = "resource-not-found",
                Detail = ntex.Message,
                Status = (int)HttpStatusCode.NotFound,
                Title = "Resource Not Found"
            });
        }
        catch (DomainException dex)
        {
            return HttpResults.BadRequest(new ProblemDetails()
            {
                Type = "domain-error",
                Detail = dex.Message,
                Status = (int)HttpStatusCode.BadRequest,
                Title = "Domain Error"
            });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unexpected error {error}", ex.Message);

            return HttpResults.InternalServerError(new ProblemDetails()
            {
                Type = "internal-server-error",
                Detail = ex.StackTrace,
                Status = (int)HttpStatusCode.InternalServerError,
                Title = ex.Message
            });
        }
    }

    public async Task<SQSBatchResponse> HandleFromSubscription<TMessage>(Func<TMessage, Task> action, SQSEvent sqsEvent)
    {
        var response = new SQSBatchResponse()
        {
            BatchItemFailures = new List<SQSBatchResponse.BatchItemFailure>()
        };

        foreach (var record in sqsEvent.Records)
        {
            try
            {
                var wrapper = JsonSerializer.Deserialize<SNSWrapper>(record.Body);

                if (wrapper == null || string.IsNullOrEmpty(wrapper.Message))
                {
                    response.BatchItemFailures.Add(new SQSBatchResponse.BatchItemFailure() { ItemIdentifier = record.MessageId });
                    continue;
                }

                var message = JsonSerializer.Deserialize<TMessage>(wrapper.Message);

                if (message == null)
                {
                    response.BatchItemFailures.Add(new SQSBatchResponse.BatchItemFailure() { ItemIdentifier = record.MessageId });
                    continue;
                }

                await action(message);
            }
            catch
            {
                response.BatchItemFailures.Add(new SQSBatchResponse.BatchItemFailure() { ItemIdentifier = record.MessageId });
            }
        }
        return response;
    }

    public class SNSWrapper
    {
        public string Message { get; set; } = null!;
    }
}
