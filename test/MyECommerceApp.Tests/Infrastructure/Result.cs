using MyECommerceApp.Infrastructure.ExceptionHandling;
using Shouldly;

namespace MyECommerceApp.Tests.Infrastructure;

public class Result
{
    public ProblemDetails? Error { get; set; }

    public Result()
    {

    }

    public Result(ProblemDetails problemDetails)
    {
        Error = problemDetails;
    }

    public void ShouldBeSuccessful()
    {
        Error.ShouldBeNull();
    }

    public void ShouldThrowError(string errorDetail)
    {
        Error.ShouldNotBeNull();
        Error.Detail.ShouldBe(errorDetail);
    }

    public void Check(string? errorDetail = null, Action<ProblemDetails>? errorAssert = null)
    {
        if (errorDetail == null)
        {
            ShouldBeSuccessful();
        }
        else
        {
            ShouldThrowError(errorDetail);
            errorAssert?.Invoke(Error!);

        }
    }
}


public class Result<TResponse>
    where TResponse : class
{
    public TResponse? Response { get; set; }
    public ProblemDetails? Error { get; set; }

    public Result()
    {

    }

    public Result(TResponse response)
    {
        Response = response;
    }

    public Result(ProblemDetails problemDetails)
    {
        Error = problemDetails;
    }

    public void ShouldBeSuccessful()
    {
        Error.ShouldBeNull();
        Response.ShouldNotBeNull();
    }

    public void ShouldThrowError(string errorDetail)
    {
        Error.ShouldNotBeNull();
        Error.Detail.ShouldBe(errorDetail);
        Response.ShouldBeNull();
    }

    public void Check(string? errorDetail = null, Action<TResponse>? successAssert = null, Action<ProblemDetails>? errorAssert = null)
    {
        if (errorDetail == null)
        {
            ShouldBeSuccessful();
            successAssert?.Invoke(Response!);
        }
        else
        {
            ShouldThrowError(errorDetail);
            errorAssert?.Invoke(Error!);

        }
    }
}

public record EmptyRequest();

public record EmptyResponse();
