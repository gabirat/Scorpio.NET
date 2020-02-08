using Scorpio.Api.DataAccess;
using Scorpio.Api.Models;

namespace Scorpio.Api.Controllers
{
    public class PositionController : CrudController<IPositionRepository, Position>
    {
        public PositionController(IPositionRepository repository) : base(repository)
        {
        }
    }
}
