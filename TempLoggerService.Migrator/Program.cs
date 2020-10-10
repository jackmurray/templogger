using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TempLoggerService.Migrator
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.SetBasePath(Directory.GetCurrentDirectory());
                    configApp.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging();
                    //services.Configure<Application>(hostContext.Configuration.GetSection("application"));
                    services.AddHostedService<MigratorService>(); 
                    
                })
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    configLogging.AddConsole(); 
                })
                .UseConsoleLifetime()
                .Build();
 
            await host.RunAsync();
        }
    }
}
