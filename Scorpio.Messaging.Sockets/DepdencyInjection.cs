using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Scorpio.Messaging.Abstractions;

namespace Scorpio.Messaging.Sockets
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Microsoft extension
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSocketClientConnection(this IServiceCollection services)
        {
            return services.AddSingleton<ISocketClient>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<SocketClient>>();
                var config = sp.GetRequiredService<IOptions<SocketConfiguration>>();

                logger.LogInformation("************************************");
                logger.LogInformation($"Socket connecting: {config.Value.Host}:{config.Value.Port}");
                logger.LogInformation("************************************");

                return new SocketClient(logger, config);
            });
        }

        /// <summary>
        /// Autofac extension
        /// </summary>
        /// <param name="builder"></param>
        public static void AddSocketClientConnection(this ContainerBuilder builder, SocketConfiguration socketConfiguration)
        {
            builder.Register<ISocketClient>(ctx =>
                {
                    var logger = ctx.Resolve<ILogger<SocketClient>>();
                    var options = Options.Create(socketConfiguration);

                    logger.LogInformation("************************************");
                    logger.LogInformation($"Socket connecting: {options.Value.Host}:{options.Value.Port}");
                    logger.LogInformation("************************************");

                    return new SocketClient(logger, options);
                })
                .SingleInstance();
        }

        /// <summary>
        /// Microsoft extension
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceCollection AddSocketClientEventBus(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<IEventBusSubscriptionManager, GenericEventBusSubscriptionManager>();
            //            services.AddSingleton<IEventBus, RabbitMqEventBus>(provider =>
            //            {
            //                var conn = provider.GetRequiredService<IRabbitMqConnection>();
            //                var logger = provider.GetRequiredService<ILogger<RabbitMqEventBus>>();
            //                var scope = provider.GetRequiredService<ILifetimeScope>();
            //                var subsManager = provider.GetRequiredService<IEventBusSubscriptionManager>();
            //
            //                var rabbitConfig = new RabbitConfig
            //                {
            //                    ExchangeName = config["RabbitMq:exchangeName"],
            //                    MyQueueName = config["RabbitMq:myQueueName"],
            //                    MessageTimeToLive = config["RabbitMq:messageTTL"]
            //                };
            //
            //                return new RabbitMqEventBus(conn, logger, scope, subsManager, rabbitConfig);
            //            });

            return services;
        }

        public static void AddSocketClientEventBus(this ContainerBuilder builder)
        {
            builder.RegisterType<GenericEventBusSubscriptionManager>()
                .As<IEventBusSubscriptionManager>()
                .SingleInstance();

            builder.Register<SocketEventBus>(ctx =>
                {
                    var client = ctx.Resolve<ISocketClient>();
                    var logger = ctx.Resolve<ILogger<SocketEventBus>>();
                    var scope = ctx.Resolve<ILifetimeScope>();
                    var subsManager = ctx.Resolve<IEventBusSubscriptionManager>();
                    return new SocketEventBus(client, logger, subsManager, scope);
                })
                .As<IEventBus>()
                .SingleInstance();
        }
    }
}
