using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TempLoggerService.ModelsCore;

namespace TempLoggerService.Api.Repositories
{
    public class TemperatureRepository : ITemperatureRepository
    {
        private ILogger<TemperatureRepository> _logger;
        private ApiContext _context;
        public TemperatureRepository(ILogger<TemperatureRepository> logger, ApiContext context)
        {
            (_logger, _context) = (logger, context);
        }

        public async Task LogTemperature(Temperature temperature)
        {
            _context.Temperatures.Add(temperature);
            await _context.SaveChangesAsync();
        }
    }
}