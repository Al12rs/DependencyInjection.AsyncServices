using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.AsyncServices.Extensions;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> to register async services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers an async service as a singleton. The service is resolved via 
    /// <see cref="ServiceProviderExtensions.GetRequiredAsyncService{TService}"/> as <see cref="Task{TService}"/>.
    /// </summary>
    /// <typeparam name="TService">The service type that implements <see cref="IAsyncServiceBase{T}"/></typeparam>
    /// <param name="services">The service collection to add the service to</param>
    /// <returns>The service collection for method chaining</returns>
    /// <remarks>
    /// <para>The service creation task is started lazily when first requested via 
    /// <see cref="ServiceProviderExtensions.GetRequiredAsyncService{TService}"/>. Once started,
    /// all subsequent requests will receive the same <see cref="Task{TService}"/> instance (singleton semantics).</para>
    /// <para>The service is automatically disposed when the DI container is disposed if it implements
    /// <see cref="IAsyncDisposable"/> or <see cref="IDisposable"/>.</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// services.AddSingletonAsync&lt;MyAsyncService&gt;();
    /// 
    /// // Later, resolve the service:
    /// var myService = await serviceProvider.GetRequiredAsyncService&lt;MyAsyncService&gt;();
    /// </code>
    /// </example>
    public static IServiceCollection AddSingletonAsync<TService>(this IServiceCollection services) 
        where TService : class, IAsyncServiceBase<TService>
    {
        return services.AddSingleton<AsyncServiceBox<TService>>(sp =>
        {
            var serviceTask = IAsyncServiceBase<TService>.CreateServiceAsync(sp);
            return new AsyncServiceBox<TService>(serviceTask);
        });
    }
    
    /// <summary>
    /// Registers an async service implementation as a singleton for a given service interface or base type.
    /// The service is resolved via <see cref="ServiceProviderExtensions.GetRequiredAsyncService{TService}"/> 
    /// as <see cref="Task{TService}"/>.
    /// </summary>
    /// <typeparam name="TService">The service interface or base class to register</typeparam>
    /// <typeparam name="TImplementation">The concrete implementation that implements <see cref="IAsyncServiceBase{T}"/></typeparam>
    /// <param name="services">The service collection to add the service to</param>
    /// <returns>The service collection for method chaining</returns>
    /// <remarks>
    /// <para>The service creation task is started lazily when first requested via 
    /// <see cref="ServiceProviderExtensions.GetRequiredAsyncService{TService}"/>. Once started,
    /// all subsequent requests will receive the same <see cref="Task{TService}"/> instance (singleton semantics).</para>
    /// <para>The service is automatically disposed when the DI container is disposed if it implements
    /// <see cref="IAsyncDisposable"/> or <see cref="IDisposable"/>.</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// services.AddSingletonAsync&lt;IMyService, MyAsyncServiceImpl&gt;();
    /// 
    /// // Later, resolve the service by interface:
    /// var myService = await serviceProvider.GetRequiredAsyncService&lt;IMyService&gt;();
    /// </code>
    /// </example>
    public static IServiceCollection AddSingletonAsync<TService, TImplementation>(this IServiceCollection services) 
        where TImplementation : class, TService, IAsyncServiceBase<TImplementation>
        where TService : class
    {
        return services.AddSingleton<AsyncServiceBox<TService>>(sp =>
        {
            var serviceImplTask = IAsyncServiceBase<TImplementation>.CreateServiceAsync(sp);
            // Cast Task<TImplementation> to Task<TService>
            var serviceTask = CastTaskToBaseType<TService, TImplementation>(serviceImplTask);
            return new AsyncServiceBox<TService>(serviceTask);
        });
    }
    
    /// <summary>
    /// Casts a <see cref="Task{TImplementation}"/> to <see cref="Task{TService}"/> where 
    /// <typeparamref name="TImplementation"/> implements or derives from <typeparamref name="TService"/>.
    /// </summary>
    /// <remarks>
    /// This is necessary because <see cref="Task{T}"/> is not covariant. We await the implementation task
    /// and return it as the base type to enable interface-based service resolution.
    /// </remarks>
    private static async Task<TService> CastTaskToBaseType<TService, TImplementation>(Task<TImplementation> task)
        where TImplementation : TService
    {
        return await task;
    }
}