using Autofac;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Scorpio.Messaging.Abstractions;
using Scorpio.Messaging.Messages;
using Scorpio.Messaging.Sockets;
using Serilog;
using Serilog.Extensions.Autofac.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;

namespace Scorpio.Examples.BarbequeueClient
{
    public class Program
    {
        private IContainer _container;
        private ILogger<Program> _logger;
        private ILogger<Program> Logger => _logger ??= _container.Resolve<ILogger<Program>>();

        static void Main(string[] args) => new Program().Run(args);

        private void Run(string[] args)
        {
            BuildContainer();
            var eventBus = _container.Resolve<IEventBus>();
            eventBus.Subscribe<RoverControlCommand, RoverControlCommandHandler>();
 
            while (true)
            {
                var msg = new RoverControlCommand(-14423.1f, 313.11312f);
                Logger.LogInformation("Publishing...");
                eventBus.Publish(msg);
                Thread.Sleep(500);
            }
        }

        private void BuildContainer()
        {
            var socketConf = new SocketConfiguration()
            {
                Host = "192.168.43.166",
                Port = 5000
            };

            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .WriteTo.Console();

            var builder = new ContainerBuilder();
            builder.RegisterSerilog(loggerConfig);
            builder.AddSocketClientConnection(socketConf);
            builder.AddSocketClientEventBus();
            builder.RegisterType<RoverControlCommandHandler>().InstancePerDependency();
            _container = builder.Build();
        }
    }

    public class RoverControlCommandHandler : IIntegrationEventHandler<RoverControlCommand>
    {
        private readonly ILogger<RoverControlCommand> _logger;

        public RoverControlCommandHandler(ILogger<RoverControlCommand> logger)
        {
            _logger = logger;
        }

        public Task Handle(RoverControlCommand @event)
        {
            _logger.LogInformation("Received:");
            _logger.LogInformation(JsonConvert.SerializeObject(@event));
            return Task.FromResult(0);
        }
    }
}
