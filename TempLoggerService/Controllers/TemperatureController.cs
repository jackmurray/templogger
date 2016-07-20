using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TempLoggerService.Models;

namespace TempLoggerService.Controllers
{
    public class TemperatureController : ApiController
    {
        // GET api/values
        /*public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }*/

        [System.Web.Http.AcceptVerbs("GET")]
        [System.Web.Http.HttpGet]
        public List<AverageTempModel> GetLast24HourTemps(Guid id)
        {
            temperaturelogEntities ent = new temperaturelogEntities();

            ObjectResult<GetTempRange_Result> getTempsLast24HoursResults = ent.GetTempsLast24Hours(id);
            return (from i in getTempsLast24HoursResults
                select new AverageTempModel() {hour = (int) i.h, avgtemp = (decimal) i.avgtemp}).ToList();
        }

        public List<TempEntry> GetLastTemps(Guid id)
        {
            temperaturelogEntities ent = new temperaturelogEntities();
            string value = Request.GetQueryNameValuePairs().FirstOrDefault(k => k.Key == "count").Value;
            int count = (!String.IsNullOrEmpty(value) ? int.Parse(value) : 10);
            var res = (from e in ent.temperatures
                where e.deviceID == id
                orderby e.timestamp descending
                select e).Take(count);

            return (from e in res
                select new TempEntry() {device = id, temp = e.value, timestamp = e.timestamp}).ToList();
        }

        public TempEntry GetLatestTemp(Guid id)
        {
            temperaturelogEntities ent = new temperaturelogEntities();
            var res = ent.GetLatestTemp(id);
            try
            {
                var i = res.First();
                return new TempEntry() {timestamp = i.timestamp.ToLocalTime(), temp = i.value, device = id};
            }
            catch
            {
                return null;
            }
        }

        // POST api/values
        public void LogTemp(TempEntry entry)
        {
            temperaturelogEntities ent = new temperaturelogEntities();
            ent.temperatures.Add(new temperature()
            {
                deviceID = entry.device,
                value = entry.temp,
                timestamp = entry.timestamp
            });

            ent.SaveChanges();
        }

        // PUT api/values/5
        /*public void Put(int id, [FromBody]string value)
        {
        }*/

        // DELETE api/values/5
        /*public void Delete(int id)
        {
        }*/
    }
}