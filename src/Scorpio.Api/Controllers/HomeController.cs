using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Scorpio.Api.Hubs;
using System.Reflection;
using System.Threading.Tasks;
using Scorpio.Api.Events;
using Scorpio.Messaging.Abstractions;

namespace Scorpio.Api.Controllers
{
    [Route("")]
    [ApiController]
    [Produces("application/json")]
    public class HomeController : ControllerBase
    {
        private readonly IHubContext<MainHub> _mainHub;
        private readonly IOptions<RabbitMqConfiguration> _rabbitConfig;
        private readonly IOptions<MongoDbConfiguration> _mongoConfig;
        private IEventBus _eventBus;

        public HomeController(IEventBus eventBus, IOptions<RabbitMqConfiguration> rabbitConfig, IOptions<MongoDbConfiguration> mongoConfig, IHubContext<MainHub> mainHub)
        {
            _eventBus = eventBus;
            _rabbitConfig = rabbitConfig;
            _mongoConfig = mongoConfig;
            _mainHub = mainHub;
        }

        [HttpGet]
        public IActionResult Index()
        {
            _mainHub.Clients.All.SendAsync("home", "dataasdasd").Wait();
            _eventBus.Publish(new FiluToSzmata("I KURWA"));

            var response = new
            {
                SwaggerDocs = "/swagger",
                Api = Assembly.GetExecutingAssembly().GetName(),
                RaabiqMqConfig = _rabbitConfig.Value,
                MongoDbConfig = _mongoConfig.Value,
            };

            return Ok(response);
        }

        public class FiluToSzmata : IntegrationEvent
        {
            public string Filu { get; set; }

            public FiluToSzmata(string filu)
            {
                Filu = filu;
            }
        }

        public class FiluToSzmataHandler : IIntegrationEventHandler<IntegrationEvent>
        {
            public Task Handle(IntegrationEvent @event)
            {
                Console.WriteLine(@event);
                return Task.FromResult(0);
            }
        }
    }
}