using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Scorpio.Api.Hubs;
using System.Reflection;

namespace Scorpio.Api.Controllers
{
    [ApiController]
    [Route("")]
    [Produces("application/json")]
    public class HomeController : ControllerBase
    {
        private readonly IHubContext<MainHub> _mainHub;
        private readonly IOptions<RabbitMqConfiguration> _rabbitConfig;
        private readonly IOptions<MongoDbConfiguration> _mongoConfig;

        public HomeController(IOptions<RabbitMqConfiguration> rabbitConfig, IOptions<MongoDbConfiguration> mongoConfig, IHubContext<MainHub> mainHub)
        {
            _rabbitConfig = rabbitConfig;
            _mongoConfig = mongoConfig;
            _mainHub = mainHub;
        }

        [HttpGet]
        public IActionResult Index()
        {
            _mainHub.Clients.All.SendAsync("data", "dataasdasd").Wait();

            var response = new
            {
                Api = Assembly.GetExecutingAssembly().GetName(),
                RaabiqMqConfig = _rabbitConfig.Value,
                MongoDbConfig = _mongoConfig.Value
            };

            return Ok(response);
        }
    }
}