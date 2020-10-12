using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;

namespace TempLoggerService.Migrator
{
    public enum ExpectedRowCount
    {
        Zero,
        AtLeastOne
    }

    public class Migrator
    {
        private ILogger<Migrator> _logger;
        private string _sourceConnectionString;
        private string _destinationConnectionString;

        private bool _isDeviceMigrationFinished;
        private bool _isTemperatureMigrationFinished;

        private SqlConnection _sourceConnection;
        private SqlConnection _destinationConnection;

        public bool IsFinished { get { return _isDeviceMigrationFinished && _isTemperatureMigrationFinished; } }

        public CancellationToken CancellationToken {get; set;}

        public Migrator(ILogger<Migrator> logger, string sourceConnectionString, string destinationConnectionString)
        {
            _logger = logger;
            _sourceConnectionString = sourceConnectionString;
            _destinationConnectionString = destinationConnectionString;
            _isDeviceMigrationFinished = false;
            _isTemperatureMigrationFinished = false;
        }

        public async Task ConnectAsync()
        {
            _sourceConnection = new SqlConnection(_sourceConnectionString);
            _destinationConnection = new SqlConnection(_destinationConnectionString);
            await _sourceConnection.OpenAsync(CancellationToken);
            await _destinationConnection.OpenAsync(CancellationToken);
            _logger.LogInformation("Connected to source and destination databases.");
        }

        public async Task ValidateSourceAsync()
        {
            if (!await ValidateTableRowCount(_sourceConnection, "dbo.devices", ExpectedRowCount.AtLeastOne) ||
                !await ValidateTableRowCount(_sourceConnection, "dbo.temperature", ExpectedRowCount.AtLeastOne))
            {
                throw new FormatException("Source database failed validation. Devices and Temperature tables must contain data.");
            }
            _logger.LogInformation("Source database validated");
        }

        public async Task ValidateDestinationAsync()
        {
            if (!await ValidateTableRowCount(_destinationConnection, "dbo.devices", ExpectedRowCount.Zero) ||
                !await ValidateTableRowCount(_destinationConnection, "dbo.temperature", ExpectedRowCount.Zero))
            {
                throw new FormatException("Destination database failed validation. Devices and Temperature tables must be empty.");
            }
            _logger.LogInformation("Destination database validated");
        }

        public async Task MigrateDevicesAsync()
        {
            string getDevicesQuery = "SELECT * FROM dbo.devices";
            using (SqlCommand cmd = new SqlCommand(getDevicesQuery, _sourceConnection))
            {
                using (var sourceDevices = await cmd.ExecuteReaderAsync())
                {
                    while (await sourceDevices.ReadAsync())
                    {
                        _logger.LogInformation("Device ID {0}, Name {1}", sourceDevices.GetGuid(0), sourceDevices.GetString(1));
                    }
                }
            }
        }

        public async Task MigrateTemperatureBatchAsync(int batchSize)
        {
            throw new NotImplementedException();
        }

        private async Task<bool> ValidateTableRowCount(SqlConnection connection, string tableName, ExpectedRowCount expectedRowCount)
        {
            string querySql = "SELECT COUNT(*) FROM " + tableName;
            using (SqlCommand cmd = new SqlCommand(querySql, connection))
            {
                int rowCount = (int)await cmd.ExecuteScalarAsync();
                return (expectedRowCount == ExpectedRowCount.Zero && rowCount == 0 ||
                        expectedRowCount == ExpectedRowCount.AtLeastOne && rowCount >= 1);
            }
        }
    }
}