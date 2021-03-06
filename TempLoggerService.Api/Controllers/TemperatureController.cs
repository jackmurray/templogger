using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TempLoggerService.ModelsCore;
using TempLoggerService.Api.Repositories;

namespace TempLoggerService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TemperatureController : ControllerBase
    {
        private readonly ILogger<TemperatureController> _logger;
        private readonly ITemperatureRepository _temperatureRepository;

        public TemperatureController(ILogger<TemperatureController> logger, ITemperatureRepository temperatureRepository)
        {
            _logger = logger;
            _temperatureRepository = temperatureRepository;
        }

        [HttpPost]
        public async Task<IActionResult> LogTemperature([FromBody]Temperature temperature)
        {
            temperature.Timestamp = DateTime.UtcNow; // Even if a timestamp was supplied we just want to override it anyway.
            await _temperatureRepository.LogTemperature(temperature);
            return Ok();
        }

        [HttpGet("GetTempRange/{deviceId}/{start}/{end}")]
        public async Task<IActionResult> GetTempRange(Guid deviceId, DateTime start, DateTime end)
        {
            return Ok(_temperatureRepository.GetHourlyAverageTemperature(deviceId, start, end));
        }

        [HttpGet("GetLatestTemp/{deviceId}")]
        public async Task<IActionResult> GetLatestTemp(Guid deviceId)
        {
            return Ok(await _temperatureRepository.GetLatestTemperature(deviceId));
        }

        [HttpGet("GetLast24HourTemperatures/{deviceId}")]
        public async Task<IActionResult> GetLast24HourTemperatures(Guid deviceId)
        {
            return Ok(_temperatureRepository.GetLast24HourTemperatures(deviceId));
        }
    }
}
