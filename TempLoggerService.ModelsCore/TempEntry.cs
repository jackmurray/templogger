using System;
using System.Collections.Generic;
using System.Linq;

namespace TempLoggerService.Models
{
    public class TempEntry
    {
        public Guid device;
        public decimal temp;
        public DateTime timestamp;
    }
}