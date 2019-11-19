using Microsoft.AspNetCore.Mvc;

namespace Scorpio.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public abstract class ScorpioController : ControllerBase
    {
    }
}
