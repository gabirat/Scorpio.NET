using Microsoft.Extensions.Options;
using Scorpio.Api.Models;

namespace Scorpio.Api.DataAccess
{
    public class SensorDataRepository : MongoRepository<SensorData>, ISensorDataRepository
    {
        public SensorDataRepository(IOptions<MongoDbConfiguration> options) : base(options)
        {
        }
    }
}
