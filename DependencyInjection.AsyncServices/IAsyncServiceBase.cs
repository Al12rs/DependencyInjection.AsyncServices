using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.AsyncServices;

/// <summary>
/// Base interface for services that require asynchronous construction and/or initialization.
/// Provides a two-step lifecycle: instance creation (via <see cref="CreateInstanceStepAsync"/>) followed by 
/// initialization (via <see cref="InitializeStepAsync"/>). Services are registered and resolved as <see cref="Task{T}"/>
/// in the DI container using <see cref="Extensions.ServiceCollectionExtensions.AddSingletonAsync{TService}"/> extension methods.
/// </summary>
/// <remarks>
/// By default, uses <see cref="ActivatorUtilities.CreateInstance{T}"/> for synchronous construction and performs no initialization.
/// Implement <see cref="IAsyncConstructable{T}"/> to customize construction (e.g., to resolve async dependencies).
/// Implement <see cref="IAsyncInitializable{T}"/> to add post-construction initialization logic.
/// </remarks>
public interface IAsyncServiceBase<T> 
    where T : IAsyncServiceBase<T>
{
    /// <summary>
    /// Creates an instance of the service. By default, uses <see cref="ActivatorUtilities.CreateInstance{T}"/>
    /// with synchronous dependency resolution from the service provider.
    /// </summary>
    /// <remarks>
    /// This method is overridden by <see cref="IAsyncConstructable{T}"/> to delegate to 
    /// <see cref="IAsyncConstructable{T}.ConstructAsync"/> for custom async construction logic (e.g., awaiting async dependencies).
    /// </remarks>
    internal static virtual Task<T> CreateInstanceStepAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        // Default: synchronous construction using DI container
        return Task.FromResult(ActivatorUtilities.CreateInstance<T>(serviceProvider));
    }
    
    /// <summary>
    /// Performs post-construction initialization on the instance. By default, returns the instance unchanged.
    /// </summary>
    /// <remarks>
    /// This method is overridden by <see cref="IAsyncInitializable{T}"/> to delegate to 
    /// <see cref="IAsyncInitializable{T}.InitializeAsync"/> for custom async initialization logic (e.g., loading data, warming caches).
    /// </remarks>
    internal Task<T> InitializeStepAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        // Default: no initialization performed
        return Task.FromResult((T)this);
    }

    /// <summary>
    /// Orchestrates the complete async service creation lifecycle by calling <see cref="CreateInstanceStepAsync"/>
    /// followed by <see cref="InitializeStepAsync"/>. This method is called by the DI container when resolving
    /// services registered via <see cref="Extensions.ServiceCollectionExtensions.AddSingletonAsync{TService}"/>.
    /// </summary>
    internal static sealed async Task<T> CreateServiceAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        var instance = await T.CreateInstanceStepAsync(serviceProvider, cancellationToken);
        return await instance.InitializeStepAsync(serviceProvider, cancellationToken);
    }
}