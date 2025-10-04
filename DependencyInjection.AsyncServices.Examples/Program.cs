using DependencyInjection.AsyncServices.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DependencyInjection.AsyncServices.Examples;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var host = new HostBuilder()
            .ConfigureServices(services => services
                .AddSingleton<MySynchronousService>()
                .AddSingletonAsync<IMyAsyncDependency, MyAsyncDependency>()
                .AddSingletonAsync<IMyAsyncService, MyAsyncBuildable>()
            )
            .ConfigureLogging(logging =>
            {
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information);
            })
            .Build();
        
        // Test the async services
        var serviceProvider = host.Services;
        
        Console.WriteLine("Resolving async services...");
        
        // Resolve and test MyAsyncBuildable
        var asyncService = await serviceProvider.GetRequiredAsyncService<IMyAsyncService>();
        Console.WriteLine("MyAsyncBuildable resolved successfully");
        await asyncService.DoWorkAsync();
        Console.WriteLine("MyAsyncBuildable work completed");
        
        // Resolve and test MyAsyncDependency
        var asyncDependency = await serviceProvider.GetRequiredAsyncService<IMyAsyncDependency>();
        Console.WriteLine("MyAsyncDependency resolved successfully");
        await asyncDependency.DoDependencyWorkAsync();
        Console.WriteLine("MyAsyncDependency work completed");
        
        Console.WriteLine("All async services tested successfully!");
        
        return 0;
    }
}