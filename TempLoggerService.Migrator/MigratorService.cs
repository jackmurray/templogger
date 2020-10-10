using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace TempLoggerService.Migrator
{
    public class MigratorService : IHostedService
    {
        private string _sourceConnectionString;
        private string _destinationConnectionString;
        private ILogger<MigratorService> _logger;
        
        public MigratorService(ILogger<MigratorService> logger, IConfiguration config)
        {
            _logger = logger;
            _sourceConnectionString = config.GetSection("ConnectionStrings").GetSection("Source").Value;
            _destinationConnectionString = config.GetSection("ConnectionStrings").GetSection("Destination").Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
 
        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}