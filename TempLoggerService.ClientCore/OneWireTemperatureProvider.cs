using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace TempLoggerService.ClientCore
{
    public class OneWireTemperatureProvider : ITemperatureProvider
    {
        private string _deviceFilePath;

        public OneWireTemperatureProvider(IConfiguration configuration)
        {
            _deviceFilePath = configuration.GetSection("TemperatureProviders").GetSection("OneWire").GetSection("File").Value;
        }

        public decimal GetTemperature()
        {
            string data = File.ReadAllText(_deviceFilePath);
            Match m = Regex.Match(data, "t=([0-9]+)", RegexOptions.None);
            return Convert.ToDecimal(m.Groups[1].Value) / 1000; //the w1_slave file outputs the temperature in milli-celcius
        }
    }
}
