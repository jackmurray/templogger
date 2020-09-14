using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TempLoggerService.ModelsCore;

namespace TempLoggerService.Api.Repositories
{
    public interface IDeviceRepository
    {
        Task<IEnumerable<Device>> GetAsync();
        Task<Device> GetAsync(Guid deviceId);
        Task<Device> GetAsync(string deviceName);
        Task<Device> CreateAsync(string deviceName);
    }
}