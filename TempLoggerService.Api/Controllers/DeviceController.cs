using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TempLoggerService.ModelsCore;

namespace TempLoggerService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceController : ControllerBase
    {
        private readonly ILogger<DeviceController> _logger;
        private readonly ApiContext _context;

        public DeviceController(ILogger<DeviceController> logger, ApiContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var d = _context.Devices.ToList();
            _logger.LogDebug("Fetched devices.");
            return Ok(new { Devices = d });
        }

        [HttpPost]
        public IActionResult Post([FromBody]string deviceName)
        {
            var d = _context.Devices.FirstOrDefault(d => d.DeviceName == deviceName);
            if (d == null)
            {
                d = new Device() { DeviceId = Guid.NewGuid(), DeviceName = deviceName };
                _context.Devices.Add(d);
                _context.SaveChanges();
            }

            return Ok(d);
        }
    }
}
