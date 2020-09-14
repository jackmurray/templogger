using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TempLoggerService.ModelsCore.StoredProcedure
{
    [NotMapped]
    public class HourlyAverageTemperature
    {
        public DateTime Date { get; set; }
        public int Hour { get; set; }
        public decimal Value { get; set; }
    }
}
