using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TempLoggerService.Models
{
    [DataContract]
    public class AverageTempModel
    {
        [DataMember]
        public int hour;

        [DataMember]
        public decimal avgtemp;
    }
}
