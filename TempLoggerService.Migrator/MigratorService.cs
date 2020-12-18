using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.ComponentModel;

namespace TempLoggerService.Migrator
{
    public enum MigratorServiceMode
    {
        Full,
        Incremental
    }
    
    public class MigratorService : IHostedService
    {
        
        private ILogger<MigratorService> _logger;
        private Migrator _migrator;
        private MigratorServiceMode _mode;
        
        public MigratorService(ILogger<MigratorService> logger, ILogger<Migrator> migratorlogger, IConfiguration config)
        {
            _logger = logger;
            string _sourceConnectionString = config.GetSection("ConnectionStrings").GetSection("Source").Value;
            string _destinationConnectionString = config.GetSection("ConnectionStrings").GetSection("Destination").Value;
            _mode = config.GetValue("Mode", MigratorServiceMode.Full);
            _migrator = new Migrator(migratorlogger, _sourceConnectionString, _destinationConnectionString);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            int batchSize = 10000;
            _logger.LogInformation("MigratorService starting");
            _migrator.CancellationToken = cancellationToken;
            await _migrator.ConnectAsync();
            Task sourceValidateTask = _migrator.ValidateSourceAsync();
            // If we're doing a full migration then we need the destination to be empty. Incremental migration requires
            // there to already be data present that we can add to.
            Task destinationValidateTask = _migrator.ValidateDestinationAsync(_mode == MigratorServiceMode.Full ? ExpectedRowCount.Zero : ExpectedRowCount.AtLeastOne);

            await sourceValidateTask;
            await destinationValidateTask;

            if (_mode == MigratorServiceMode.Full) // No need to do this on an incremental migration.
                await _migrator.MigrateDevicesAsync();

            DateTime migrateRecordsAfter;
            if (_mode == MigratorServiceMode.Incremental)
            {
                migrateRecordsAfter = await _migrator.GetTimestampOfMostRecentTemperatureAsync();
            }
            else
            {
                migrateRecordsAfter = DateTime.MinValue;
            }

            await _migrator.MigrateTemperaturesAsync(batchSize, migrateRecordsAfter);

            return;
        }
 
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}