using Microsoft.Extensions.Options;
using Scorpio.Api.Models;

namespace Scorpio.Api.DataAccess
{
    public class UiConfigurationRepository : MongoRepository<UiConfiguration>, IUiConfigurationRepository
    {
        public UiConfigurationRepository(IOptions<MongoDbConfiguration> options) : base(options)
        {
        }
    }
}
