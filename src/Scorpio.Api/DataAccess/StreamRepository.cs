using Microsoft.Extensions.Options;
using Scorpio.Api.Models;

namespace Scorpio.Api.DataAccess
{
    public class StreamRepository : MongoRepository<Stream>, IStreamRepository
    {
        public StreamRepository(IOptions<MongoDbConfiguration> options) : base(options)
        {
        }
    }
}
