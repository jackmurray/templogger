using System;
using System.Collections.Generic;
using System.Text;

namespace TempLoggerService.ClientCore
{
    interface ITemperatureLogClient
    {
        public Guid GetDeviceGuidByName(string name);
        public void SetTemperature(Guid devID, decimal temperature);
        public void SetTemperature(string device, decimal temperature);
    }
}
