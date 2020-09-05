using System;
using System.Threading.Tasks;

namespace TempLoggerService.ClientCore
{
    interface ITemperatureLogClient
    {
        public Task<Guid> GetDeviceGuidByName(string name);
        public Task SetTemperature(Guid devID, decimal temperature);
        public Task SetTemperature(string device, decimal temperature);
    }
}
