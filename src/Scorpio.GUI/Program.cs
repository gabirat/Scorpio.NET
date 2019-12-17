using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using RabbitMQ.Client;
using Scorpio.Gamepad.Processors;
using Scorpio.GUI.Streaming;
using Scorpio.Messaging.Abstractions;
using Scorpio.Messaging.RabbitMQ;
using System;
using System.Threading;
using System.Windows.Forms;
using Scorpio.Instrumentation.Vivotek;

namespace Scorpio.GUI
{
    internal static class Program
    {
        private static ILogger<MainForm> _logger;
        private static IContainer _container;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                Application.ThreadException += Application_ThreadException;

                var builder = new ContainerBuilder();
                var services = PopulateServices(builder);
                _container = services.Build();

#pragma warning disable 618
                SetupLogger();
#pragma warning restore 618

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                _logger = _container.Resolve<ILogger<MainForm>>();

                Application.Run(_container.Resolve<MainForm>());
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private static ContainerBuilder PopulateServices(ContainerBuilder builder)
        {
            builder.RegisterType<MainForm>().SingleInstance();

            var config = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", optional: false)
              .Build();

            builder.RegisterInstance(config)
                .As<IConfiguration>()
                .SingleInstance()
                .ExternallyOwned();

            var vivotekConfig = new CameraConfigModel();
            config.GetSection("cameras").Bind(vivotekConfig);

            builder.RegisterInstance(vivotekConfig)
                .As<CameraConfigModel>()
                .SingleInstance()
                .ExternallyOwned();

            builder.RegisterType<LoggerFactory>()
                .As<ILoggerFactory>()
                .SingleInstance();

            builder.RegisterGeneric(typeof(Logger<>))
                .As(typeof(ILogger<>))
                .SingleInstance();

            SetupRabbitMqConnection(builder, config);
            SetupRabbitMqEventBus(builder, config);

            builder.RegisterGeneric(typeof(ExponentialGamepadProcessor<,>))
                .As(typeof(IGamepadProcessor<,>))
                .InstancePerDependency();

            builder.RegisterType<CyclicTimer>()
                .InstancePerDependency();

            builder.RegisterType<GStreamerLauncher>()
                .SingleInstance();

            builder.RegisterType<VivotekDomeCameraController>()
                .InstancePerDependency();

            return builder;
        }

        private static void SetupRabbitMqConnection(ContainerBuilder builder, IConfiguration config)
        {
            builder.Register<IRabbitMqConnection>(ctx =>
                {
                    var logger = ctx.Resolve<ILogger<RabbitMqConnection>>();

                    var factory = new ConnectionFactory
                    {
                        HostName = config["rabbitMq:host"],
                        Port = config.GetValue<int>("rabbitMq:port"),
                        UserName = config["rabbitMq:userName"],
                        Password = config["rabbitMq:password"],
                        VirtualHost = config["rabbitMq:virtualHost"]
                    };

                    logger.LogInformation("************************************");
                    logger.LogInformation($"RabbitMQ factory created: {factory.UserName}:{factory.Password}@{factory.HostName}:{factory.Port}{factory.VirtualHost}");
                    logger.LogInformation("************************************");

                    return new RabbitMqConnection(factory, logger);
                })
                .SingleInstance();
        }

        private static void SetupRabbitMqEventBus(ContainerBuilder builder, IConfiguration config)
        {
            builder.RegisterType<GenericEventBusSubscriptionManager>()
                .As<IEventBusSubscriptionManager>()
                .SingleInstance();

            builder.Register<RabbitMqEventBus>(ctx =>
            {
                var conn = ctx.Resolve<IRabbitMqConnection>();
                var logger = ctx.Resolve<ILogger<RabbitMqEventBus>>();
                var scope = ctx.Resolve<ILifetimeScope>();
                var subsManager = ctx.Resolve<IEventBusSubscriptionManager>();

                var rabbitConfig = new RabbitConfig
                {
                    ExchangeName = config["rabbitMq:exchangeName"],
                    MyQueueName = config["rabbitMq:myQueueName"],
                    MessageTimeToLive = config["rabbitMq:messageTTL"],
                };

                return new RabbitMqEventBus(conn, logger, scope, subsManager, rabbitConfig);
            })
            .As<IEventBus>()
            .SingleInstance();
        }

        [Obsolete("YES I KNOW ITS OBSOLETE", false)]
        private static void SetupLogger()
        {
            var loggerFactory = _container.Resolve<ILoggerFactory>();
            loggerFactory.AddNLog(new NLogProviderOptions
            {
                CaptureMessageProperties = true,
                CaptureMessageTemplates = true
            });
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
            => HandleException(e.Exception);

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
            => HandleException(e.ExceptionObject as Exception);

        private static void HandleException(Exception ex)
        {
            _logger.LogError(ex?.Message, ex?.ToString());
        }
    }
}
