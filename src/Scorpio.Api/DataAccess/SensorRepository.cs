using Microsoft.Extensions.Options;
using Scorpio.Api.Models;

namespace Scorpio.Api.DataAccess
{
    public class SensorRepository : MongoRepository<Sensor>, ISensorRepository
    {
        public SensorRepository(IOptions<MongoDbConfiguration> options) : base(options)
        { 
        }
    }
}
