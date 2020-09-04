using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TempLoggerService.Models;
using Newtonsoft.Json;
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

            while (true)
            {
                try
                {
                    _client.SetTemperature(id, GetTempFrom1WireSensor());

                    Thread.Sleep(30000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }            
        }

        private static decimal GetTempFrom1WireSensor()
        {
            string data = File.ReadAllText("/sys/bus/w1/devices/28-000006a00e95/w1_slave");
            Match m = Regex.Match(data, "t=([0-9]+)", RegexOptions.None);
            return Convert.ToDecimal(m.Groups[1].Value) / 1000; //the w1_slave file outputs the temperature in milli-celcius
        }
    }
}
