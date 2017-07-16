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

        public ActionResult Index(string id)
        {
            ViewBag.devices = GetDevices();
            if (String.IsNullOrEmpty(id))
                id = "rpi1";

            ViewBag.devname = id;
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
            items.Add(new SelectListItem { Value = "", Text = "", Selected = true}); //the blank one to go at the top.
            devs = devs.OrderBy(d => d.name);
            foreach (var dev in devs)
            {
                items.Add(new SelectListItem { Value = dev.id.ToString(), Text = dev.name});
            }

            return new SelectList(items, "Value", "Text");
        }

    }
}
