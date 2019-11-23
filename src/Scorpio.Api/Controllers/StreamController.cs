using Scorpio.Api.DataAccess;
using Scorpio.Api.Models;

namespace Scorpio.Api.Controllers
{
    public class StreamController : CrudController<IStreamRepository, Stream>
    {
        public StreamController(IStreamRepository streamRepository) : base(streamRepository)
        {
        }
    }
}
