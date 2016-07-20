﻿using System;
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

namespace TempLoggerService.Client
{
    class Program
    {
        private static HttpClient client;

        private static Guid GetDevice(string name)
        {
            Guid id = Guid.Empty;
            HttpResponseMessage response = client.GetAsync(String.Format("api/device/{0}", name)).Result;
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                HttpResponseMessage createresp = client.PostAsJsonAsync("api/device/", name).Result;
                if (createresp.StatusCode != HttpStatusCode.OK)
                    throw new Exception("Unable to create device: " + createresp.StatusCode);
                else
                    id = createresp.Content.ReadAsAsync<Guid>().Result;
            }
            else if (response.StatusCode == HttpStatusCode.OK)
            {
                id = response.Content.ReadAsAsync<Guid>().Result;
            }
            else
                throw new Exception("Unable to fetch device GUID");

            if (id == Guid.Empty)
                throw new Exception("For some reason we still failed to get the device GUID.");

            return id;
        }

        private static void Setup()
        {
            client = new HttpClient();

            // New code:
            client.BaseAddress = new Uri("http://core2/TempLoggerService/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        static void Main(string[] args)
        {          
            string hostname = Dns.GetHostName();
            Console.WriteLine("Hostname: {0}", hostname);
            Guid id = GetDevice(hostname);           

            while (true)
            {
                TempEntry n = new TempEntry()
                {
                    temp = GetTemp(),
                    device = id,
                    timestamp = DateTime.UtcNow
                };

                try
                {
                    HttpResponseMessage postresp = client.PostAsJsonAsync("api/temperature/LogTemp", n).Result;
                    if (!postresp.IsSuccessStatusCode)
                        throw new Exception("Failed to log temperature.");

                    Thread.Sleep(30000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }            
        }

        private static decimal GetTemp()
        {
            string data = File.ReadAllText("/sys/bus/w1/devices/28-000006a00e95/w1_slave");
            Match m = Regex.Match(data, "t=([0-9]+)", RegexOptions.None);
            return Convert.ToDecimal(m.Groups[1].Value) / 1000; //the w1_slave file outputs the temperature in milli-celcius
        }
    }
}
