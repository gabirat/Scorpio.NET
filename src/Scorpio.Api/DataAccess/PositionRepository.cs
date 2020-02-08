using Microsoft.Extensions.Options;
using Scorpio.Api.Models;

namespace Scorpio.Api.DataAccess
{
    public class PositionRepository : MongoRepository<Position>, IPositionRepository
    {
        public PositionRepository(IOptions<MongoDbConfiguration> options) : base(options)
        {
        }
    }
}