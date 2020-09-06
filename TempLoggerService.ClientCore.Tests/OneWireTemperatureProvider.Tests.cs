using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using TempLoggerService.ClientCore;

namespace TempLoggerService.ClientCore.Tests
{
    public class Tests
    {
        private IConfiguration config;

        [OneTimeSetUp]
        public void Setup()
        {
            config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.UnitTest.json", optional: false, reloadOnChange: false)
                .Build();
        }

        [Test]
        public void ParseTemperatureFromFile()
        {
            OneWireTemperatureProvider p = new OneWireTemperatureProvider(config);
            Assert.AreEqual(1.1, p.GetTemperature());
        }
    }
}