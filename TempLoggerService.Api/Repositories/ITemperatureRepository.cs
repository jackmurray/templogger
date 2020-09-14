using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TempLoggerService.ModelsCore;
using TempLoggerService.ModelsCore.StoredProcedure;

namespace TempLoggerService.Api.Repositories
{
    public interface ITemperatureRepository
    {
        Task LogTemperature(Temperature t);
        IAsyncEnumerable<HourlyAverageTemperature> GetHourlyAverageTemperature(Guid DeviceId, DateTime Start, DateTime End);
        Task<Temperature> GetLatestTemperature(Guid DeviceId);
    }
}