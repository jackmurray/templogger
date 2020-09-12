using System.Threading.Tasks;
using TempLoggerService.ModelsCore;

namespace TempLoggerService.Api.Repositories
{
    public interface ITemperatureRepository
    {
        Task LogTemperature(Temperature t);
    }
}