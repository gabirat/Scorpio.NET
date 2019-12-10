using Autofac;
using Microsoft.Extensions.Logging;
using NLog.Windows.Forms;
using RabbitMQ.Client;
using Scorpio.Gamepad.IO;
using Scorpio.Gamepad.IO.Args;
using Scorpio.Gamepad.Processors;
using Scorpio.Gamepad.Processors.Mixing;
using Scorpio.GUI.Configuration;
using Scorpio.Messaging.Abstractions;
using Scorpio.Messaging.Messages;
using Scorpio.Messaging.RabbitMQ;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace Scorpio.GUI
{
    public partial class MainForm : Form
    {
        private readonly ILifetimeScope _iocFactory;
        private readonly ILogger<MainForm> _logger;
        private IEventBus _eventBus;
        private IGamepadProcessor<RoverMixer, RoverProcessorResult> _roverGamepadProcessor;
        private IGamepadPoller _roverGamepad;

        public MainForm(ILifetimeScope iocFactory)
        {
            InitializeComponent();

            _iocFactory = iocFactory;
            _logger = _iocFactory.Resolve<ILogger<MainForm>>();

            var cfg = ConfigurationManager.GetSection("cameraConfig") as CameraConfigSection;
            SetupGamepads();
            SetupMessageBus();

            var sender = _iocFactory.Resolve<Sender>();
            sender.Start(40);

            base.Load += (_, __) => RichTextBoxTarget.ReInitializeAllTextboxes(this); // Refresh NLog RichTextBox
        }

        private void SetupGamepads()
        {
            var pollerThreadSleepTime = int.Parse(ConfigurationManager.AppSettings["gamepadUpdateFrequency"]);
            _roverGamepad = new GamepadPoller(0, pollerThreadSleepTime);
            _roverGamepad.GamepadStateChanged += roverGamepad_GamepadStateChanged;
            _roverGamepad.StartPolling();
            _roverGamepadProcessor = _iocFactory.Resolve<IGamepadProcessor<RoverMixer, RoverProcessorResult>>();
        }

        private void roverGamepad_GamepadStateChanged(object sender, GamepadEventArgs e)
        {
            var processed = _roverGamepadProcessor.Process(e.Gamepad);
            _logger.LogInformation(processed.Direction.ToString());
            // todo timer to send data periodically
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

            var connLogger = _iocFactory.Resolve<ILogger<RabbitMqConnection>>();
            var busLogger = _iocFactory.Resolve<ILogger<RabbitMqEventBus>>();
            var subsManager = _iocFactory.Resolve<IEventBusSubscriptionManager>();
            var config = new RabbitConfig
            {
                ExchangeName = ConfigurationManager.AppSettings["rabbitExchange"],
                MyQueueName = ConfigurationManager.AppSettings["rabbitMyQueue"],
                MessageTimeToLive = ConfigurationManager.AppSettings["rabbitMessageTTL"],
            };

            var conn = new RabbitMqConnection(factory, connLogger);
            conn.OnConnected += Conn_OnConnected;
            conn.OnDisconnected += Conn_OnDisconnected;
            _eventBus = new RabbitMqEventBus(conn, busLogger, _iocFactory, subsManager, config);
        }

        private void Conn_OnDisconnected(object sender, EventArgs e)
        {
            _logger.LogInformation("disc");
        }

        private void Conn_OnConnected(object sender, EventArgs e)
        {
            _logger.LogInformation("connected");
        }
    }
}
