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
    public class DeviceController : ControllerBase
    {
        private readonly ILogger<DeviceController> _logger;
        private readonly IDeviceRepository _repo;

        public DeviceController(ILogger<DeviceController> logger, IDeviceRepository deviceRepository)
        {
            _logger = logger;
            _repo = deviceRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var d = await _repo.GetAsync();
            return Ok(new { Devices = d });
        }


        // Route Constraint to match GUIDs. Not sure what would happen if a device had a GUID as it's name but it'll probably end up confusing the routing
        // as it'll think the name is the ID because it looks like a GUID.
        [HttpGet("{deviceId:guid}")]
        public async Task<IActionResult> GetById(Guid deviceId)
        {
            return Ok(await _repo.GetAsync(deviceId));
        }

        [HttpGet("{deviceName}")]
        public async Task<IActionResult> GetByName(string deviceName)
        {
            return Ok(await _repo.GetAsync(deviceName));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]string deviceName)
        {
            var d = await _repo.CreateAsync(deviceName);

            return CreatedAtAction(nameof(GetById), new { deviceId = d.DeviceId }, d);
        }
    }
}
