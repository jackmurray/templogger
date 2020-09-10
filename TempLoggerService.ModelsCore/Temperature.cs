using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TempLoggerService.ModelsCore
{
    public class Temperature
    {
        [Key]
        [Required]
        // This column is just so that we can have a clustered index on a value that will increment monotonically. SQL Server Clustered Indexes
        // (which the primary key must be) are stored sorted on-disk, so using e.g. a GUID for the key is bad for performance because it constantly
        // has to re-order everything. This ID column isn't referenced by anything so it could freely be reset as desired e.g. in a database merge.
        public int TemperatureId { get; set; }
        public Guid DeviceId { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal Value { get; set; }

        public Device Device { get; set; }
    }
}
