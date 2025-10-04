using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.AsyncServices.Extensions;

public static class ServiceProviderExtensions
{
    /// <summary>
    /// Resolves an async service registered using AddSingletonAsync.
    /// </summary>
    /// <typeparam name="TService">The service type to resolve</typeparam>
    /// <param name="serviceProvider">The service provider</param>
    /// <param name="cancellationToken">Cancellation token (currently not used but reserved for future use)</param>
    /// <returns>The resolved service instance</returns>
    public static Task<TService> GetRequiredAsyncService<TService>(
        this IServiceProvider serviceProvider, 
        CancellationToken cancellationToken = default) 
        where TService : class
    {
        return serviceProvider.GetRequiredService<Task<TService>>();
    }
}