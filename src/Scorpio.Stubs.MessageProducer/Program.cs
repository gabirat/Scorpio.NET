using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Scorpio.Messaging.Abstractions;
using Scorpio.Messaging.RabbitMQ;
using Scorpio.Stubs.MessageProducer.Events;
using Serilog;
using System;
using System.Threading;
using Newtonsoft.Json;

namespace Scorpio.Stubs.MessageProducer
{
    public class Program
    {
        private static IServiceProvider _serviceProvider;

        static void Main(string[] args)
        {
            AddLogger();
            RegisterServices();

            var logger = _serviceProvider.GetRequiredService<ILogger<Program>>();
            var conn = _serviceProvider.GetRequiredService<IEventBus>();
            while (true)
            {
                var @event = new UpdateRoverPositionEvent("dupa", "dupsko");
                logger.LogInformation(JsonConvert.SerializeObject(@event));
                conn.Publish(@event);
                Thread.Sleep(50);
            }

            Console.ReadKey();
        }

        private static void AddLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
        }

        private static void RegisterServices()
        {
            var services = new ServiceCollection();

            services.AddLogging(configure => configure.AddSerilog());
            services.AddRabbitMqConnection();
            services.AddRabbitMqEventBus();

            var builder = new ContainerBuilder();
            builder.Populate(services);
            var appContainer = builder.Build();
            _serviceProvider = new AutofacServiceProvider(appContainer);
        }
    }
}
