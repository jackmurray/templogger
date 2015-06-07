using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TempLoggerService.Models
{
    public class TempEntry
    {
        public Guid device;
        public decimal temp;
        public DateTime timestamp;
    }
}