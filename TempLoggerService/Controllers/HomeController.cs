using System;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TempLoggerService.Models;

namespace TempLoggerService.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.devices = GetDevices();
            return View();
        }

        protected IEnumerable<SelectListItem> GetDevices()
        {
            var ent = new temperaturelogEntities();
            var devs = ent
                        .devices
                        .Select(x =>
                                new
                                {
                                    id = x.deviceID,
                                    name = x.deviceName
                                });

            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var dev in devs)
            {
                items.Add(new SelectListItem { Value = dev.id.ToString(), Text = dev.name});
            }

            return new SelectList(items, "Value", "Text");
        }

    }
}
