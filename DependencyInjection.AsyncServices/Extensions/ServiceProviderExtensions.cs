using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.AsyncServices.Extensions;

/// <summary>
/// Extension methods for <see cref="IServiceProvider"/> to resolve async services.
/// </summary>
public static class ServiceProviderExtensions
{
    /// <summary>
    /// Resolves an async service registered via <see cref="ServiceCollectionExtensions.AddSingletonAsync{TService}"/>.
    /// Returns a <see cref="Task{TService}"/> that represents the asynchronous construction and initialization of the service.
    /// </summary>
    /// <typeparam name="TService">The service type to resolve</typeparam>
    /// <param name="serviceProvider">The service provider</param>
    /// <returns>A task that completes when the service is fully constructed and initialized, yielding the service instance</returns>
    /// <remarks>
    /// <para>For singleton services, the first call initiates the async construction and initialization process.
    /// All subsequent calls return the same task instance, ensuring singleton semantics.</para>
    /// <para>The returned task may already be completed (if resolved previously) or may be in progress.
    /// Multiple concurrent calls are safe and will all await the same underlying task.</para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown if the service is not registered in the DI container</exception>
    public static Task<TService> GetRequiredAsyncService<TService>(
        this IServiceProvider serviceProvider) 
        where TService : class
    {
        var asyncServiceBox = serviceProvider.GetRequiredService<AsyncServiceBox<TService>>();
        return asyncServiceBox.GetServiceAsync();
    }
}