namespace DependencyInjection.AsyncServices;


// internal interface IAsyncServiceBox : IAsyncDisposable
// {
//     
// }


internal interface IAsyncServiceBox<TService> : IAsyncDisposable
    where TService : class
{
    Task<TService> GetServiceAsync();
} 

/// <summary>
/// Wrapper for async services that provides lifecycle management and disposal support.
/// This class encapsulates the <see cref="Task{TService}"/> returned by 
/// <see cref="IAsyncServiceBase{T}.CreateServiceAsync"/> and ensures proper disposal
/// of the service when the DI container is disposed.
/// </summary>
/// <typeparam name="TService">The service type being wrapped</typeparam>
/// <remarks>
/// This is an internal implementation detail of the async service infrastructure.
/// Consumers should use <see cref="Extensions.ServiceProviderExtensions.GetRequiredAsyncService{TService}"/>
/// to resolve async services rather than interacting with this class directly.
/// </remarks>
internal class AsyncServiceBox<TService> : IAsyncServiceBox<TService>, IAsyncDisposable 
    where TService : class
{
    private readonly Task<TService> _serviceTask;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of <see cref="AsyncServiceBox{TService}"/> with the service creation task.
    /// </summary>
    /// <param name="serviceTask">The task that creates and initializes the service</param>
    /// <param name="serviceProvider"></param>
    public AsyncServiceBox(Task<TService> serviceTask, IServiceProvider serviceProvider)
    {
        _serviceTask = serviceTask;
        _serviceProvider = serviceProvider;
    }
    
    /// <summary>
    /// Gets the task that represents the asynchronous service creation and initialization.
    /// </summary>
    /// <returns>The task containing the fully initialized service instance</returns>
    public Task<TService> GetServiceAsync() => _serviceTask;
    
    /// <summary>
    /// Disposes the service if it implements <see cref="IAsyncDisposable"/> or <see cref="IDisposable"/>.
    /// Only attempts disposal if the service task completed successfully.
    /// </summary>
    /// <remarks>
    /// This method is called automatically when the DI container is disposed, ensuring
    /// proper cleanup of async services. Failed or incomplete service tasks are not disposed.
    /// </remarks>
    public async ValueTask DisposeAsync()
    {
        // Only dispose if the service was successfully created
        if (_serviceTask.IsCompletedSuccessfully)
        {
            // Dispose dependers first
            // foreach (var depender in _dependers)
            // {
            //     await depender.DisposeAsync();
            // }
            
            var service = await _serviceTask;
            
            // Prefer async disposal over synchronous
            switch (service)
            {
                case IAsyncDisposable asyncDisposable:
                    await asyncDisposable.DisposeAsync();
                    break;
                case IDisposable disposable:
                    disposable.Dispose();
                    break;
            }
        }
    }
    
}

internal sealed class AsyncServiceBoxProxy<TService, TImpl> : IAsyncServiceBox<TService>
    where TService : class
    where TImpl : class, TService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IAsyncServiceBox<TImpl> _implServiceBox;

    public AsyncServiceBoxProxy(IAsyncServiceBox<TImpl> implServiceBox, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _implServiceBox = implServiceBox;
    }

    public async Task<TService> GetServiceAsync()
    {
        return await _implServiceBox.GetServiceAsync();
    }

    public ValueTask DisposeAsync()
    {
        return _implServiceBox.DisposeAsync();
    }
}
