using DependencyInjection.AsyncServices.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DependencyInjection.AsyncServices;

public interface IMyAsyncService
{
    Task DoWorkAsync();
}

public class MyAsyncBuildable : IMyAsyncService, IAsyncInitializable<MyAsyncBuildable>, IAsyncConstructable<MyAsyncBuildable>
{

    private readonly ILogger<MyAsyncBuildable> _logger;
    private readonly IMyAsyncDependency _myAsyncDependency;

    public static async Task<MyAsyncBuildable> ConstructAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<MyAsyncBuildable>>();
        var myAsyncDependency = await serviceProvider.GetRequiredAsyncService<IMyAsyncDependency>();
        return new MyAsyncBuildable(logger, myAsyncDependency);
    }
    
    public MyAsyncBuildable(ILogger<MyAsyncBuildable> logger, IMyAsyncDependency myAsyncDependency)
    {
        _myAsyncDependency = myAsyncDependency;
        _logger = logger;
    }

    public async Task<MyAsyncBuildable> InitializeAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken);
        return this;
    }

    public async Task DoWorkAsync()
    {
        await Task.Delay(TimeSpan.FromMilliseconds(500));
    }


}

public interface IMyAsyncDependency
{
    Task DoDependencyWorkAsync();
}

public class MyAsyncDependency : IMyAsyncDependency, IAsyncInitializable<MyAsyncDependency>
{
    private readonly ILogger<MyAsyncDependency> _logger;

    public MyAsyncDependency(ILogger<MyAsyncDependency> logger)
    {
        _logger = logger;
    }

    public async Task<MyAsyncDependency> InitializeAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken);
        return this;
    }

    public Task DoDependencyWorkAsync()
    {
        return Task.Delay(TimeSpan.FromMilliseconds(500));
    }
}


public interface IMySynchronousService
{
}
public class MySynchronousService : IMySynchronousService
{
    private readonly ILogger<MySynchronousService> _logger;
    public MySynchronousService(ILogger<MySynchronousService> logger)
    {
        _logger = logger;
    }

    public Task DoWorkAsync()
    {
        return Task.Delay(TimeSpan.FromMilliseconds(500));
    }
}