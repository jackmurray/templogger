using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TempLoggerService.ModelsCore
{
    public class Device
    {
        [Key]
        [Required]
        public Guid DeviceId { get; set; }
        [Required]
        public string DeviceName { get; set; }
        //public List<Temperature> Temperatures { get; set; }
    }
}
