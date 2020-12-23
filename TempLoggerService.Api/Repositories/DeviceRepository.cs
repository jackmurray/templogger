using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TempLoggerService.ModelsCore;
using Microsoft.EntityFrameworkCore;

namespace TempLoggerService.Api.Repositories
{
    public class DeviceRepository : IDeviceRepository
    {
        private ILogger<DeviceRepository> _logger;
        private ApiContext _context;
        public DeviceRepository(ILogger<DeviceRepository> logger, ApiContext context)
        {
            (_logger, _context) = (logger, context);
        }

        public async Task<IEnumerable<Device>> GetAsync()
        {
            return (await _context.Devices.ToListAsync()).OrderBy(d => d.DeviceName);
        }

        public async Task<Device> GetAsync(Guid deviceId)
        {
            return await _context.Devices.FirstOrDefaultAsync(d => d.DeviceId == deviceId);
        }

        public async Task<Device> GetAsync(string deviceName)
        {
            return await _context.Devices.FirstOrDefaultAsync(d => d.DeviceName == deviceName);
        }

        public async Task<Device> CreateAsync(string deviceName)
        {
            var d = _context.Devices.FirstOrDefault(d => d.DeviceName == deviceName);
            if (d == null)
            {
                d = new Device() { DeviceId = Guid.NewGuid(), DeviceName = deviceName };
                _context.Devices.Add(d);
                await _context.SaveChangesAsync();
            }
            return d;
        }
    }
}