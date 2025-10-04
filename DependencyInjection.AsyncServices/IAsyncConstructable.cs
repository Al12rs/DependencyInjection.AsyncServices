namespace DependencyInjection.AsyncServices;

/// <summary>
/// Interface for services that require custom asynchronous construction logic.
/// Implement <see cref="ConstructAsync"/> to define construction logic that runs automatically
/// during service resolution. Do not call <see cref="ConstructAsync"/> manually.
/// </summary>
/// <remarks>
/// Use this when you need to resolve async dependencies (services registered via 
/// <see cref="Extensions.ServiceCollectionExtensions.AddSingletonAsync{TService}(IServiceCollection)"/>) before constructing the instance.
/// For post-construction initialization, implement <see cref="IAsyncInitializable{T}"/> instead.
/// </remarks>
public interface IAsyncConstructable<T> : IAsyncServiceBase<T>
    where T : IAsyncConstructable<T>
{
    static Task<T> IAsyncServiceBase<T>.CreateInstanceStepAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        // Delegate to ConstructAsync during the construction phase
        return T.ConstructAsync(serviceProvider, cancellationToken);
    }
    
    /// <summary>
    /// Custom factory method for creating instances of this type.
    /// This method is automatically invoked by the framework during service resolution - do not call it directly.
    /// </summary>
    /// <remarks>
    /// <para>Implement this when you need async operations during construction, such as resolving async dependencies via
    /// <see cref="Extensions.ServiceProviderExtensions.GetRequiredAsyncService{TService}"/>.</para>
    /// <para>This method should focus on construction only. For post-construction initialization (e.g., loading data, warming caches),
    /// implement <see cref="IAsyncInitializable{T}.InitializeAsync"/> instead.</para>
    /// </remarks>
    public static abstract Task<T> ConstructAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default);
}