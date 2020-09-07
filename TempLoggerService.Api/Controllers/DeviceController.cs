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
            var model = _context.Devices.ToList();
            _logger.LogDebug("Fetched devices.");
            return Ok(new { Devices = model });
        }
    }
}
