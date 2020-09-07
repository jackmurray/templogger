using System;
using System.Collections.Generic;
using System.Text;

namespace TempLoggerService.ModelsCore
{
    public class Temperature
    {
        public Device Device { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal Value { get; set; }
    }
}
