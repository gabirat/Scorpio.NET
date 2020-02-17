using Autofac;
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
                var autofac = sp.GetRequiredService<ILifetimeScope>();

                logger.LogInformation("**********************************************************");
                logger.LogInformation($"Socket trying to connect: {config.Value.Host}:{config.Value.Port}");
                logger.LogInformation("**********************************************************");

                return new SocketClient(autofac, logger, config);
            });
        }

        /// <summary>
        /// Autofac extension
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="socketConfiguration"></param>
        public static ContainerBuilder AddSocketClientConnection(this ContainerBuilder builder, SocketConfiguration socketConfiguration)
        {
             builder.Register<ISocketClient>(ctx =>
                {
                    var logger = ctx.Resolve<ILogger<SocketClient>>();
                    var options = Options.Create(socketConfiguration);
                    var autofac = ctx.Resolve<ILifetimeScope>();

                    logger.LogInformation("**********************************************************");
                    logger.LogInformation($"Socket trying to connect: {options.Value.Host}:{options.Value.Port}");
                    logger.LogInformation("**********************************************************");

                    return new SocketClient(autofac, logger, options);
                })
                .SingleInstance();

             return builder;
        }

        /// <summary>
        /// Microsoft extension
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSocketClientEventBus(this IServiceCollection services)
        {
            services.AddSingleton<IEventBusSubscriptionManager, GenericEventBusSubscriptionManager>();
            services.AddSingleton<IEventBus, SocketEventBus>(provider =>
            {
                var client = provider.GetRequiredService<ISocketClient>();
                var logger = provider.GetRequiredService<ILogger<SocketEventBus>>();
                var scope = provider.GetRequiredService<ILifetimeScope>();
                var subsManager = provider.GetRequiredService<IEventBusSubscriptionManager>();

                return new SocketEventBus(client, logger, subsManager, scope);
            });

            return services;
        }

        /// <summary>
        /// Autofac extension
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ContainerBuilder AddSocketClientEventBus(this ContainerBuilder builder)
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

            return builder;
        }
    }
}
