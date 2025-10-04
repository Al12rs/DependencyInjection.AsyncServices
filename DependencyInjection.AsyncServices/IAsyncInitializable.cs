namespace DependencyInjection.AsyncServices;

/// <summary>
/// Interface for services that require asynchronous initialization after construction.
/// Implement <see cref="InitializeAsync"/> to define initialization logic that runs automatically
/// during service resolution. Do not call <see cref="InitializeAsync"/> manually.
/// </summary>
public interface IAsyncInitializable<T> : IAsyncServiceBase<T>
    where T : IAsyncInitializable<T>
{
    Task<T> IAsyncServiceBase<T>.InitializeStepAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        // Delegate to InitializeAsync during the post-construction phase
        return InitializeAsync(serviceProvider, cancellationToken);
    }
    
    /// <summary>
    /// Performs asynchronous initialization operations after the instance is constructed.
    /// This method is automatically invoked by the framework during service resolution - do not call it directly.
    /// </summary>
    /// <remarks>
    /// Use this for operations like loading configuration, warming caches, or establishing connections.
    /// The service is resolved via <see cref="Extensions.ServiceProviderExtensions.GetRequiredAsyncService{TService}"/>,
    /// which handles calling this method automatically.
    /// </remarks>
    Task<T> InitializeAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default);
}