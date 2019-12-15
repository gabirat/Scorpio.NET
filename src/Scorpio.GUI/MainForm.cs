using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Windows.Forms;
using RabbitMQ.Client;
using Scorpio.Gamepad.IO;
using Scorpio.Gamepad.IO.Args;
using Scorpio.Gamepad.Processors;
using Scorpio.Gamepad.Processors.Mixing;
using Scorpio.Messaging.Abstractions;
using Scorpio.Messaging.Messages;
using Scorpio.Messaging.RabbitMQ;
using System;
using System.Windows.Forms;

namespace Scorpio.GUI
{
    public partial class MainForm : Form
    {
        private readonly IConfiguration _config;
        private readonly ILifetimeScope _iocFactory;
        private readonly ILogger<MainForm> _logger;
        private IEventBus _eventBus;

        public MainForm(ILifetimeScope iocFactory, IConfiguration config, ILogger<MainForm> logger)
        {
            InitializeComponent();
            base.Load += (_, __) => RichTextBoxTarget.ReInitializeAllTextboxes(this); // Refresh NLog RichTextBox

            ucRoverGamepad1.Setup(iocFactory.Resolve<ILifetimeScope>());
            _config = config;
            _iocFactory = iocFactory;
            _logger = logger;

            SetupStreamControl();

            //SetupMessageBus();

            var sender = _iocFactory.Resolve<CyclicTimer>();
            sender.Start(100);
            sender.ElapsedAction = () =>
            {
                Console.WriteLine("ELAPSED");
            };


            this.AutoScaleMode = AutoScaleMode.Dpi;
        }

        private void SetupStreamControl()
        {
            // Maybe build this dynamically basing on config?
            ucStreamControl1.Autofac = _iocFactory.Resolve<ILifetimeScope>();
            ucStreamControl2.Autofac = _iocFactory.Resolve<ILifetimeScope>();
            ucStreamControl3.Autofac = _iocFactory.Resolve<ILifetimeScope>();
            ucStreamControl4.Autofac = _iocFactory.Resolve<ILifetimeScope>();
            ucStreamControl1.CameraId = "cam1";
            ucStreamControl2.CameraId = "cam2";
            ucStreamControl3.CameraId = "cam3";
            ucStreamControl4.CameraId = "cam4";

            ucVivotekController1.Autofac = _iocFactory.Resolve<ILifetimeScope>();
            ucVivotekController1.VivotekId = "vivotek1";
        }


        private void SetupMessageBus()
        {
            var factory = new ConnectionFactory
            {
                HostName = _config["rabbitMq:host"],
                Port = _config.GetValue<int>("rabbitMq:port"),
                UserName = _config["rabbitMq:userName"],
                Password = _config["rabbitMq:password"],
                VirtualHost = _config["rabbitMq:virtualHost"]
            };

            var connLogger = _iocFactory.Resolve<ILogger<RabbitMqConnection>>();
            var busLogger = _iocFactory.Resolve<ILogger<RabbitMqEventBus>>();
            var subsManager = _iocFactory.Resolve<IEventBusSubscriptionManager>();
            var config = new RabbitConfig
            {
                ExchangeName = _config["rabbitMq:exchangeName"],
                MyQueueName = _config["rabbitMq:myQueueName"],
                MessageTimeToLive = _config["rabbitMq:messageTTL"],
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

        private void ucVivotekController1_Load(object sender, EventArgs e)
        {

        }

        private void ucStreamControl4_Load(object sender, EventArgs e)
        {

        }
    }
}
