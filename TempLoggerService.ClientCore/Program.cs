using System;
using System.Net;
using System.Threading;
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

        private static void Configuration()
        {
            config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .Build();
        }

        private static void ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<ITemperatureLogClient>(new TemperatureLogClient(new Uri("http://templogger.corp.c0rporation.com/TempLoggerService/")));
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
                    _client.SetTemperature(args[1], Decimal.Parse(args[2]));
                    return;
                }
            }

            string hostname = Dns.GetHostName();
            Console.WriteLine("Hostname: {0}", hostname);
            Guid id = _client.GetDeviceGuidByName(hostname); //For caching basically, so that we don't fetch it every time since it won't change.
            ITemperatureProvider provider = serviceProvider.GetService<ITemperatureProvider>();

            while (true)
            {
                try
                {
                    _client.SetTemperature(id, provider.GetTemperature());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
                finally
                {
                    Thread.Sleep(30000);
                }
            }            
        }
    }
}
