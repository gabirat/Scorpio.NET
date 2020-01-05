using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Scorpio.Api.Controllers
{
    [Route("")]
    [ApiController]
    [Produces("application/json")]
    public class HomeController : ControllerBase
    {
        private readonly IOptions<RabbitMqConfiguration> _rabbitConfig;
        private readonly IOptions<MongoDbConfiguration> _mongoConfig;

        public HomeController(IOptions<RabbitMqConfiguration> rabbitConfig, IOptions<MongoDbConfiguration> mongoConfig)
        {
            _rabbitConfig = rabbitConfig;
            _mongoConfig = mongoConfig;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var response = new
            {
                SwaggerDocs = "/swagger",
                Api = Assembly.GetExecutingAssembly().GetName(),
                RaabiqMqConfig = _rabbitConfig.Value,
                MongoDbConfig = _mongoConfig.Value,
            };

            return Ok(response);
        }
    }
}