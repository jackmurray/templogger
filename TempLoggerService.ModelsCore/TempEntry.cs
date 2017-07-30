using System;
using System.Collections.Generic;
using System.Linq;

namespace TempLoggerService.Models
{
    public class TempEntry
    {
        public Guid device { get; set; }
        public decimal temp { get; set; }
        public DateTime timestamp { get; set; }
    }
}