using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace TempLoggerService.Controllers
{
    public class DeviceController : ApiController
    {
        // GET api/device
        public IEnumerable<Guid> Get()
        {
            temperaturelogEntities ent = new temperaturelogEntities();
            return ent.devices.Select(d => d.deviceID);
        }

        // GET api/device/5
        public Guid Get(string id)
        {
            temperaturelogEntities ent = new temperaturelogEntities();
            var firstOrDefault = ent.devices.FirstOrDefault(d => d.deviceName == id);
            if (firstOrDefault != null)
                return firstOrDefault.deviceID;
            else
                throw new HttpResponseException(HttpStatusCode.NotFound);
        }

        // POST api/device
        public Guid Post([FromBody]string id)
        {
            temperaturelogEntities ent = new temperaturelogEntities();
            var firstOrDefault = ent.devices.FirstOrDefault(d => d.deviceName == id);
            if (firstOrDefault == null)
            {
                device d = ent.devices.Create();
                d.deviceID = Guid.NewGuid();
                d.deviceName = id;
                ent.devices.Add(d);
                ent.SaveChanges();
                return d.deviceID;
            }
            else
            {
                return firstOrDefault.deviceID; //already exists, so just output that.
            }
        }

        // DELETE api/device/5
        /*public void Delete(int id)
        {
        }*/
    }
}
