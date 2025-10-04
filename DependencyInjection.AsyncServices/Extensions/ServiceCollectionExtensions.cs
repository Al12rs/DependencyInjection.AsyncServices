using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.AsyncServices.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers an async service as a singleton. The service will be resolved as Task&lt;TService&gt;.
    /// </summary>
    /// <typeparam name="TService">The service type that implements IAsyncServiceBase</typeparam>
    public static IServiceCollection AddSingletonAsync<TService>(this IServiceCollection services) 
        where TService : class, IAsyncServiceBase<TService>
    {
        return services.AddSingleton<Task<TService>>(sp => 
            IAsyncServiceBase<TService>.CreateServiceAsync(sp));
    }
    
    /// <summary>
    /// Registers an async service implementation as a singleton for a given service type.
    /// The service will be resolved as Task&lt;TService&gt;.
    /// </summary>
    /// <typeparam name="TService">The service interface or base class</typeparam>
    /// <typeparam name="TImplementation">The concrete implementation that implements IAsyncServiceBase</typeparam>
    public static IServiceCollection AddSingletonAsync<TService, TImplementation>(this IServiceCollection services) 
        where TImplementation : class, TService, IAsyncServiceBase<TImplementation>
        where TService : class
    {
        return services.AddSingleton<Task<TService>>(async sp => 
            await IAsyncServiceBase<TImplementation>.CreateServiceAsync(sp));
    }
}