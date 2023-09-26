using Xunit.Abstractions;

namespace MyECommerceApp.Tests.Infrastructure;

public abstract class Dsl
{
    protected readonly TimeoutMonitor _timeoutMonitor;
    protected readonly ITestOutputHelper _output;
    protected readonly static TimeSpan _defaultTimeout = TimeSpan.FromSeconds(30);
    protected readonly static TimeSpan _defaultInterval = TimeSpan.FromSeconds(30);
    protected Dsl(ITestOutputHelper output)
    {
        _timeoutMonitor = new TimeoutMonitor();
        _output = output;
    }

    public Task WaitFor(Func<Task> taskFactory, TimeSpan timeout, TimeSpan interval) => _timeoutMonitor.RunUntil(taskFactory, timeout, interval);
}
