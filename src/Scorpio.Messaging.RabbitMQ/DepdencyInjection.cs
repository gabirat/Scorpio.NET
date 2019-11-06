using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Scorpio.Messaging.Abstractions;

namespace Scorpio.Messaging.RabbitMQ
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRabbitMqConnection(this IServiceCollection services, IConfiguration config)
        {
            return services.AddSingleton<IRabbitMqConnection>(sp =>
            {
                var hostname = config["RabbitMq:host"];
                
                var logger = sp.GetRequiredService<ILogger<RabbitMqConnection>>();

                logger.LogInformation("************************************");
                logger.LogInformation("Rabbit connecting to: " + hostname);
                logger.LogInformation("************************************");

                var factory = new ConnectionFactory
                {
                    HostName = hostname,
                    Port = 5672,
                    UserName = "guest",
                    Password = "guest",
                    VirtualHost = "/"
                };

                return new RabbitMqConnection(factory, logger);
            });
        }

        public static IServiceCollection AddRabbitMqEventBus(this IServiceCollection services)
        {
            services.AddSingleton<IEventBusSubscriptionManager, GenericEventBusSubscriptionManager>();

            services.AddSingleton<IEventBus, RabbitMqEventBus>(provider =>
            {
                var conn = provider.GetRequiredService<IRabbitMqConnection>();
                var logger = provider.GetRequiredService<ILogger<RabbitMqEventBus>>();
                var scope = provider.GetRequiredService<ILifetimeScope>();
                var subsManager = provider.GetRequiredService<IEventBusSubscriptionManager>();

                return new RabbitMqEventBus(conn, logger, scope, subsManager, "Hub");
            });

            return services;
        }
    }
}
