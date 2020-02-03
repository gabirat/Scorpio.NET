﻿using Autofac;
using Microsoft.Extensions.Logging;
using NLog.Windows.Forms;
using Scorpio.Messaging.Abstractions;
using Scorpio.Messaging.RabbitMQ;
using System.Windows.Forms;
using Scorpio.Messaging.Sockets;

namespace Scorpio.GUI
{
    public partial class MainForm : Form
    {
        private readonly ILifetimeScope _iocFactory;
        private readonly ILogger<MainForm> _logger;
        private readonly IEventBus _eventBus;

        public MainForm(ILifetimeScope iocFactory, ILogger<MainForm> logger)
        {
            InitializeComponent();

            _iocFactory = iocFactory;
            _eventBus = _iocFactory.Resolve<IEventBus>();
            _logger = logger;

            SetupStreamControl();
            SetupGamepadControls();
            SetupMessaging();

            this.AutoScaleMode = AutoScaleMode.Dpi;
            base.Load += (_, __) => RichTextBoxTarget.ReInitializeAllTextboxes(this); // Refresh NLog RichTextBox
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

        private void SetupGamepadControls()
        {
            ucRoverGamepad1.Setup(_iocFactory.Resolve<ILifetimeScope>());
        }

        private void SetupMessaging()
        { 
//            var conn = (RabbitMqConnection)_iocFactory.Resolve<IRabbitMqConnection>();
//
//            conn.OnConnected += (_, __) => _logger.LogInformation("RabbitMQ connected!");
//            conn.OnDisconnected += (_, __) => _logger.LogError("RabbitMQ disconnected");

        }
    }
}
