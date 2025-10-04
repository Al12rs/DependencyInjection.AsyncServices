namespace DependencyInjection.AsyncServices;

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
internal sealed class AsyncServiceBox<TService> : IAsyncDisposable 
    where TService : class
{
    private readonly Task<TService> _serviceTask;
    
    /// <summary>
    /// Initializes a new instance of <see cref="AsyncServiceBox{TService}"/> with the service creation task.
    /// </summary>
    /// <param name="serviceTask">The task that creates and initializes the service</param>
    public AsyncServiceBox(Task<TService> serviceTask)
    {
        _serviceTask = serviceTask;
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