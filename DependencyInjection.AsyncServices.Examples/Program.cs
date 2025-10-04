using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DependencyInjection.AsyncServices.Examples;

public class Program
{
    [STAThread]
    public static int Main(string[] args)
    {
        var host = new HostBuilder()
            .ConfigureServices(services => services
                .AddSingleton<MySynchronousService>()
            )
            .Build();
        
        return 0;
    }
}