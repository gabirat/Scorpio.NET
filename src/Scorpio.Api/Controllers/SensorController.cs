using Scorpio.Api.DataAccess;
using Scorpio.Api.Models;

namespace Scorpio.Api.Controllers
{
    public class SensorController : CrudController<ISensorRepository, Sensor>
    {
        public SensorController(ISensorRepository sensorRepository) : base(sensorRepository)
        {
        }
    }
}