using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using TempLoggerService.ModelsCore;
using System.Collections.Generic;
using System.Linq;

namespace TempLoggerService.Migrator
{
    public enum ExpectedRowCount
    {
        Zero,
        AtLeastOne
    }

    public class Migrator
    {
        private const int MAX_INSERT_SIZE = 5;

        private ILogger<Migrator> _logger;
        private string _sourceConnectionString;
        private string _destinationConnectionString;

        private SqlConnection _sourceConnection;
        private SqlConnection _destinationConnection;


        public CancellationToken CancellationToken {get; set;}

        public Migrator(ILogger<Migrator> logger, string sourceConnectionString, string destinationConnectionString)
        {
            _logger = logger;
            _sourceConnectionString = sourceConnectionString;
            _destinationConnectionString = destinationConnectionString;
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
            if (!await ValidateTableRowCount(_sourceConnection, "dbo.device", ExpectedRowCount.AtLeastOne) ||
                !await ValidateTableRowCount(_sourceConnection, "dbo.temperature", ExpectedRowCount.AtLeastOne))
            {
                throw new FormatException("Source database failed validation. Devices and Temperature tables must contain data.");
            }
            _logger.LogInformation("Source database validated");
        }

        public async Task ValidateDestinationAsync()
        {
            if (!await ValidateTableRowCount(_destinationConnection, "dbo.devices", ExpectedRowCount.Zero) ||
                !await ValidateTableRowCount(_destinationConnection, "dbo.temperatures", ExpectedRowCount.Zero))
            {
                throw new FormatException("Destination database failed validation. Devices and Temperature tables must be empty.");
            }
            _logger.LogInformation("Destination database validated");
        }

        public async Task MigrateDevicesAsync()
        {
            string getDevicesQuery = "SELECT * FROM dbo.device ORDER BY deviceName ASC";
            var sourceDevicesList = new List<Device>();
            using (SqlCommand cmd = new SqlCommand(getDevicesQuery, _sourceConnection))
            {
                using (var sourceDevices = await cmd.ExecuteReaderAsync())
                {
                    while (await sourceDevices.ReadAsync())
                    {
                        Device d = new Device() { DeviceId = sourceDevices.GetGuid(0), DeviceName = sourceDevices.GetString(1) };
                        _logger.LogInformation("Found device ID {0}, Name {1}", d.DeviceId, d.DeviceName);
                        sourceDevicesList.Add(d);
                    }
                }
            }
            _logger.LogInformation("Inserting {0} devices into the destination database", sourceDevicesList.Count);

            using (var trans = await _destinationConnection.BeginTransactionAsync(CancellationToken) as SqlTransaction)
            {
                try
                {
                    using (var command = _destinationConnection.CreateCommand())
                    {
                        string insertDeviceQuery = "INSERT INTO dbo.devices (DeviceId, DeviceName) VALUES (@devId, @devName)";
                        command.CommandText = insertDeviceQuery;
                        command.Transaction = trans;
                        command.Connection = _destinationConnection;
                        command.Prepare();

                        foreach (Device d in sourceDevicesList)
                        {
                            command.Parameters.AddWithValue("@devId", d.DeviceId);
                            command.Parameters.AddWithValue("@devName", d.DeviceName);
                            await command.ExecuteNonQueryAsync(CancellationToken);
                            command.Parameters.Clear();
                        }
                    }

                    await trans.CommitAsync(CancellationToken);
                    _logger.LogInformation("Device migration complete.");
                }
                catch
                {
                    await trans.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task MigrateTemperaturesAsync(int batchSize)
        {
            string getTemperaturesQuery = "SELECT * FROM dbo.temperature ORDER BY timestamp ASC";
            var temperatureBatch = new List<Temperature>(batchSize); //might as well re-use the model class to store the results.
            int remainingRowCount = await GetTableRowCount(_sourceConnection, "dbo.temperature");
            _logger.LogInformation("Migrating {0} temperature records", remainingRowCount);
            using (SqlCommand cmd = new SqlCommand(getTemperaturesQuery, _sourceConnection))
            {
                cmd.CommandTimeout = 120; // need a longer timeout because sorting the temperatures by timestamp can be slow
                using (var temperatureReader = await cmd.ExecuteReaderAsync())
                {
                    while (await temperatureReader.ReadAsync())
                    {
                        temperatureBatch.Add(new Temperature()
                        {
                            DeviceId = temperatureReader.GetGuid(0),
                            Timestamp = temperatureReader.GetDateTime(1),
                            Value = temperatureReader.GetDecimal(2)
                        });

                        // If we have filled a batch, then migrate it and start over.
                        if (temperatureBatch.Count == batchSize)
                        {
                            await MigrateTemperatureBatchAsync(temperatureBatch);
                            temperatureBatch.Clear();
                            remainingRowCount -= batchSize;
                            _logger.LogInformation("{0} temperature records remaining.", remainingRowCount);
                        }
                    }

                    // If there's a partial batch left at the end (i.e. there weren't an exact multiple of batchSize records)
                    // then migrate those. No need to clear the batch as it'll go out of scope anyway.
                    if (temperatureBatch.Count > 0)
                    {
                        await MigrateTemperatureBatchAsync(temperatureBatch);
                    }

                    _logger.LogInformation("All temperature records migrated.");
                }
            }
        }

        private async Task MigrateTemperatureBatchAsync(List<Temperature> temperatureBatch)
        {
            using (var trans = await _destinationConnection.BeginTransactionAsync(CancellationToken) as SqlTransaction)
            {
                try
                {
                    using (var command = _destinationConnection.CreateCommand())
                    {
                        command.Transaction = trans;
                        command.Connection = _destinationConnection;

                        int inserted = 0;

                        while (inserted != temperatureBatch.Count)
                        {
                            int remaining = temperatureBatch.Count - inserted;
                            if (remaining >= MAX_INSERT_SIZE) // if we have enough to insert a full group then do that
                            {
                                await InsertTemperatureData(command, temperatureBatch.Skip(inserted).Take(MAX_INSERT_SIZE).ToList());
                                inserted += MAX_INSERT_SIZE;
                            }
                            else // if we can't fill a group then insert the remaining rows one at a time
                            {
                                foreach (Temperature t in temperatureBatch.Skip(inserted))
                                {
                                    await InsertTemperatureData(command, t);
                                    inserted += 1;
                                }
                            }
                        }
                    }

                    await trans.CommitAsync(CancellationToken);
                }
                catch
                {
                    await trans.RollbackAsync();
                    throw;
                }
            }
        }

        private async Task<bool> ValidateTableRowCount(SqlConnection connection, string tableName, ExpectedRowCount expectedRowCount)
        {
            int rowCount = await GetTableRowCount(connection, tableName);
            return (expectedRowCount == ExpectedRowCount.Zero && rowCount == 0 ||
                    expectedRowCount == ExpectedRowCount.AtLeastOne && rowCount >= 1);
        }

        private async Task<int> GetTableRowCount(SqlConnection connection, string tableName)
        {
            string querySql = "SELECT COUNT(*) FROM " + tableName;
            using (SqlCommand cmd = new SqlCommand(querySql, connection))
            {
                return (int)await cmd.ExecuteScalarAsync();
            }
        }

        private async Task InsertTemperatureData(SqlCommand command, List<Temperature> temperatures)
        {
            const string insertTemperatureQuery = "INSERT INTO dbo.Temperatures (DeviceId, Timestamp, Value) VALUES (@devId0, @timestamp0, @value0)," +
                                                  "(@devId1, @timestamp1, @value1), (@devId2, @timestamp2, @value2), (@devId3, @timestamp3, @value3), (@devId4, @timestamp4, @value4)";
            if (temperatures.Count != MAX_INSERT_SIZE)
                throw new DataMisalignedException("Attempted to use group insert command with an invalid group size.");

            command.CommandText = insertTemperatureQuery;
            for (int i = 0; i < MAX_INSERT_SIZE; i++)
            {
                Temperature t = temperatures[i];
                command.Parameters.AddWithValue($"@devId{i}", t.DeviceId);
                command.Parameters.AddWithValue($"@timestamp{i}", t.Timestamp);
                command.Parameters.AddWithValue($"@value{i}", t.Value);
            }

            await command.ExecuteNonQueryAsync(CancellationToken);
            command.Parameters.Clear();
        }

        private async Task InsertTemperatureData(SqlCommand command, Temperature temperature)
        {
            const string insertTemperatureQuery = "INSERT INTO dbo.Temperatures (DeviceId, Timestamp, Value) VALUES (@devId, @timestamp, @value)";
            command.CommandText = insertTemperatureQuery;
            command.Parameters.AddWithValue("@devId", temperature.DeviceId);
            command.Parameters.AddWithValue("@timestamp", temperature.Timestamp);
            command.Parameters.AddWithValue("@value", temperature.Value);

            await command.ExecuteNonQueryAsync(CancellationToken);
            command.Parameters.Clear();
        }
    }
}