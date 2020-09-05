using System;
using System.Collections.Generic;
using System.Text;

namespace TempLoggerService.ClientCore
{
    interface ITemperatureProvider
    {
        public decimal GetTemperature();
    }
}
