using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace TempLoggerService.Migrator
{
    public class MigratorService : IHostedService
    {
        
        private ILogger<MigratorService> _logger;
        private Migrator _migrator;
        
        public MigratorService(ILogger<MigratorService> logger, ILogger<Migrator> migratorlogger, IConfiguration config)
        {
            _logger = logger;
            string _sourceConnectionString = config.GetSection("ConnectionStrings").GetSection("Source").Value;
            string _destinationConnectionString = config.GetSection("ConnectionStrings").GetSection("Destination").Value;
            _migrator = new Migrator(migratorlogger, _sourceConnectionString, _destinationConnectionString);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("MigratorService starting");
            _migrator.CancellationToken = cancellationToken;
            await _migrator.ConnectAsync();
            await _migrator.ValidateSourceAsync();
            await _migrator.ValidateDestinationAsync();
            await _migrator.MigrateDevicesAsync();
            while (!cancellationToken.IsCancellationRequested && !_migrator.IsFinished)
            {
                await _migrator.MigrateTemperatureBatchAsync(1000);
            }
            return;
        }
 
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}