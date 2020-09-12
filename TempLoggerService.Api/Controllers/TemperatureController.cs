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
    public class TemperatureController : ControllerBase
    {
        private readonly ILogger<TemperatureController> _logger;
        private readonly ApiContext _context;

        public TemperatureController(ILogger<TemperatureController> logger, ApiContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost]
        public IActionResult LogTemperature([FromBody]Temperature temperature)
        {
            temperature.Timestamp = DateTime.UtcNow; // Even if a timestamp was supplied we just want to override it anyway.
            _context.Temperatures.Add(temperature);
            _context.SaveChanges();
            return Ok();
        }
    }
}
