using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Scorpio.Api.Hubs;
using System.Reflection;

namespace Scorpio.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IHubContext<MainHub> _mainHub;
        private readonly IOptions<RabbitMqConfiguration> _options;

        public HomeController(IOptions<RabbitMqConfiguration> options, IHubContext<MainHub> mainHub)
        {
            _options = options;
            _mainHub = mainHub;
        }

        [HttpGet]
        public IActionResult Index()
        {
            _mainHub.Clients.All.SendAsync("data", "dataasdasd").Wait();

            var response = new
            {
                Api = Assembly.GetExecutingAssembly().GetName(),
                Config = _options
            };

            return Ok(response);
        }
    }
}