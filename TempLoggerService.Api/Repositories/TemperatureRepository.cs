using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using TempLoggerService.ModelsCore;
using TempLoggerService.ModelsCore.StoredProcedure;

namespace TempLoggerService.Api.Repositories
{
    public class TemperatureRepository : ITemperatureRepository
    {
        private ILogger<TemperatureRepository> _logger;
        private ApiContext _context;
        public TemperatureRepository(ILogger<TemperatureRepository> logger, ApiContext context)
        {
            (_logger, _context) = (logger, context);
        }

        public async IAsyncEnumerable<HourlyAverageTemperature> GetHourlyAverageTemperature(Guid DeviceId, DateTime Start, DateTime End)
        {
            DbDataReader reader = await _context.CreateStoredProcedure()
                .WithCommand("dbo.GetTempRange")
                .WithParameter(new SqlParameter("@Device", System.Data.SqlDbType.UniqueIdentifier) { Value = DeviceId })
                .WithParameter(new SqlParameter("@Start", System.Data.SqlDbType.DateTime) { Value = Start })
                .WithParameter(new SqlParameter("@End", System.Data.SqlDbType.DateTime) { Value = End })
                .ExecuteAsync();

            // Note that if you inspect the reader in the debugger then its results will be enumerated and the row position will
            // be changed, which will make it appear like rows are missing.
            while (await reader.ReadAsync())
            {
                yield return new HourlyAverageTemperature()
                {
                    Date = (DateTime)reader["d"],
                    Hour = (int)reader["h"],
                    Value = (decimal)reader["avgtemp"]
                };
            }
        }

        public async Task LogTemperature(Temperature temperature)
        {
            _context.Temperatures.Add(temperature);
            await _context.SaveChangesAsync();
        }

        public async Task<Temperature> GetLatestTemperature(Guid DeviceId)
        {
            return  await _context.Temperatures
                    .Where(t => t.DeviceId == DeviceId)
                    .OrderByDescending(t => t.Timestamp)
                    .FirstOrDefaultAsync();
        }
    }
}