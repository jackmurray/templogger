using System;
using System.Net;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using TempLoggerService.ClientCore;

namespace TempLoggerService.Client
{
    class Program
    {
        private static ITemperatureLogClient _client;
        private static IServiceProvider serviceProvider;


        private static void ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<ITemperatureLogClient>(new TemperatureLogClient(new Uri("http://templogger.corp.c0rporation.com/TempLoggerService/")));
            //services.AddSingleton<ITemperatureLogClient>(new TemperatureLogClient(new Uri("http://localhost:11317")));
            services.AddSingleton<ITemperatureProvider>(new OneWireTemperatureProvider("/sys/bus/w1/devices/28-000006a00e95/w1_slave"));

            serviceProvider = services.BuildServiceProvider();
        }


        static void Main(string[] args)
        {
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

                    Thread.Sleep(30000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }            
        }
    }
}
