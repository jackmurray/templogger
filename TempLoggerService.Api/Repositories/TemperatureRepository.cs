using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
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
    }
}