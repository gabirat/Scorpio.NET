using Autofac;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Scorpio.Gamepad.IO;
using Scorpio.Gamepad.IO.Args;
using Scorpio.Gamepad.Processors;
using Scorpio.Gamepad.Processors.Mixing;
using Scorpio.GUI.Configuration;
using Scorpio.Messaging.Abstractions;
using Scorpio.Messaging.RabbitMQ;
using Serilog;
using Serilog.Extensions.Autofac.DependencyInjection;
using System;
using System.Configuration;
using System.Windows.Forms;
using Scorpio.Messaging.Messages;

namespace Scorpio.GUI
{
    public partial class MainForm : Form
    {
        private IContainer _autofac;
        private IEventBus _eventBus;
        private IGamepadProcessor<RoverMixer, RoverProcessorResult> _roverGamepadProcessor;
        private IGamepadPoller _roverGamepad;

        public MainForm()
        {
            InitializeComponent();
            var cfg = ConfigurationManager.GetSection("cameraConfig") as CameraConfigSection;
            var gamepadUpdateFrequency = int.Parse(ConfigurationManager.AppSettings["gamepadUpdateFrequency"]);
            SetupDepedencyInjection();
            SetupGamepads(gamepadUpdateFrequency);
            SetupMessageBus();
        }

        private void SetupDepedencyInjection()
        {
            var builder = new ContainerBuilder();
            builder.RegisterSerilog(new LoggerConfiguration());
            builder.RegisterType<GenericEventBusSubscriptionManager>()
                .As<IEventBusSubscriptionManager>()
                .SingleInstance();

            _autofac = builder.Build();
        }

        private void SetupGamepads(int gamepadUpdateFrequency)
        {
            _roverGamepad = new GamepadPoller(0, gamepadUpdateFrequency);
            _roverGamepad.GamepadStateChanged += roverGamepad_GamepadStateChanged;
            _roverGamepad.StartPolling();
            _roverGamepadProcessor = new ExponentialGamepadProcessor();
        }

        private void roverGamepad_GamepadStateChanged(object sender, GamepadEventArgs e)
        {
            var processed = _roverGamepadProcessor.Process(e.Gamepad);
            Console.WriteLine(processed.Direction);
            _eventBus?.Publish(new RoverControlEvent(processed.Acceleration, processed.Direction));
        }

        private void SetupMessageBus()
        {
            var factory = new ConnectionFactory
            {
                HostName = ConfigurationManager.AppSettings["rabbitHost"],
                Port = int.Parse(ConfigurationManager.AppSettings["rabbitPort"]),
                UserName = ConfigurationManager.AppSettings["rabbitUser"],
                Password = ConfigurationManager.AppSettings["rabbitPassword"],
                VirtualHost = ConfigurationManager.AppSettings["rabbitVhost"]
            };

            var connLogger = _autofac.Resolve<ILogger<RabbitMqConnection>>();
            var busLogger = _autofac.Resolve<ILogger<RabbitMqEventBus>>();
            var subsManager = _autofac.Resolve<IEventBusSubscriptionManager>();
            var config = new RabbitConfig
            {
                ExchangeName = ConfigurationManager.AppSettings["rabbitExchange"],
                MyQueueName = ConfigurationManager.AppSettings["rabbitMyQueue"],
                MessageTimeToLive = ConfigurationManager.AppSettings["rabbitMessageTTL"],
            };

            var conn = new RabbitMqConnection(factory, connLogger);
            _eventBus = new RabbitMqEventBus(conn, busLogger, _autofac, subsManager, config);
        }
    }
}
