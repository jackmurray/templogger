using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TempLoggerService.ClientCore;

namespace TempLoggerService.Client
{
    class Program
    {
        private static ITemperatureLogClient _client;
        private static IServiceProvider serviceProvider;
        private static IConfiguration config;
        private static string HostingEnvironment;

        private static void Configuration()
        {
            var env_var = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (env_var != null)
            {
                HostingEnvironment = env_var;
            }
            else if (System.Diagnostics.Debugger.IsAttached)
            {
                HostingEnvironment = "Development";
            }
            else
            {
                HostingEnvironment = "Production";
            }
            config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{HostingEnvironment}.json", optional: true, reloadOnChange: true)
                .Build();
        }

        private static void ConfigureServices()
        {
            var services = new ServiceCollection();
            var apiServer = config.GetSection("ApiServer").Value;
            Console.WriteLine(apiServer);

            services.AddSingleton<ITemperatureLogClient>(new TemperatureLogClient(new Uri(apiServer)));
            //services.AddSingleton<ITemperatureLogClient>(new TemperatureLogClient(new Uri("http://localhost:11317")));
            services.AddSingleton<ITemperatureProvider, OneWireTemperatureProvider>();
            services.AddSingleton(config);

            serviceProvider = services.BuildServiceProvider();
        }


        static void Main(string[] args)
        {
            Configuration();
            ConfigureServices();

            _client = serviceProvider.GetService<ITemperatureLogClient>();

            if (args.Length > 0)
            {
                if (args[0] == "--set")
                {
                    _client.SetTemperature(args[1], Decimal.Parse(args[2])).Wait(); //cannot await because Main() can't be async.
                    return;
                }
            }

            MainAsync().Wait();
        }

        private static async Task MainAsync()
        {
            string hostname = Dns.GetHostName();
            Console.WriteLine("Hostname: {0}", hostname);
            Guid id = await _client.GetDeviceGuidByName(hostname); //For caching basically, so that we don't fetch it every time since it won't change.
            ITemperatureProvider provider = serviceProvider.GetService<ITemperatureProvider>();

            while (true)
            {
                try
                {
                    await _client.SetTemperature(id, provider.GetTemperature());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
                finally
                {
                    await Task.Delay(30000);
                }
            }
        }
    }
}
