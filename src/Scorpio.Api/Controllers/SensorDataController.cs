using Microsoft.AspNetCore.Mvc;
using Scorpio.Api.DataAccess;
using Scorpio.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scorpio.Api.Controllers
{
    public class SensorDataController : CrudController<ISensorDataRepository, SensorData>
    {
        public SensorDataController(ISensorDataRepository dataRepository) : base(dataRepository)
        {
        }

        [HttpGet("sensorKey/{sensorKey}")]
        [ProducesResponseType(typeof(List<SensorData>), 200)]
        public async Task<IActionResult> GetBySensorKey(string sensorKey)
        {
            var result = await Repository.GetManyFiltered(x => x.SensorKey == sensorKey);
            return Ok(result);
        }
    }
}
