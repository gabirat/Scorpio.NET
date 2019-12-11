using Autofac;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Scorpio.Gamepad.Processors;
using Scorpio.Messaging.Abstractions;
using System;
using System.Threading;
using System.Windows.Forms;

namespace Scorpio.GUI
{
    static class Program
    {
        private static readonly ILogger<MainForm> logger;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.ThreadException += Application_ThreadException;

            var builder = new ContainerBuilder();
            var services = PopulateServices(builder);
            var container = services.Build();

            SetupLogger(container);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(container.Resolve<MainForm>());
        }

        static ContainerBuilder PopulateServices(ContainerBuilder builder)
        {
            builder.RegisterType<MainForm>().SingleInstance();

            builder.RegisterType<LoggerFactory>()
                .As<ILoggerFactory>()
                .SingleInstance();

            builder.RegisterGeneric(typeof(Logger<>))
                .As(typeof(ILogger<>))
                .SingleInstance();

            builder.RegisterType<GenericEventBusSubscriptionManager>()
                .As<IEventBusSubscriptionManager>()
                .SingleInstance();

            builder.RegisterGeneric(typeof(ExponentialGamepadProcessor<,>))
                .As(typeof(IGamepadProcessor<,>))
                .InstancePerDependency();

            builder.RegisterType<CyclicTimer>()
                .InstancePerDependency();

            return builder;
        }

        [Obsolete("YES I KNOW ITS OBSOLETE", false)]
        private static void SetupLogger(IContainer container)
        {
            var loggerFactory = container.Resolve<ILoggerFactory>();
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
            logger.LogError(ex?.Message, ex?.ToString());
        }
    }
}
