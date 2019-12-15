using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Scorpio.Gamepad.IO;
using Scorpio.Gamepad.IO.Args;
using Scorpio.Gamepad.Processors;
using Scorpio.Gamepad.Processors.Mixing;
using Scorpio.GUI.Utils;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Scorpio.GUI.Controls
{
    public partial class ucRoverGamepad : UserControl
    {
        private ILifetimeScope _autofac;
        public ILifetimeScope Autofac
        {
            get
            {
                if (_autofac is null && !DesignMode)
                    throw new ArgumentNullException("Autofac was not initialized in this control.");
                return _autofac;
            }
            set => _autofac = value;
        }

        private bool _isStarted;
        private GamepadPoller _poller;
        private IGamepadProcessor<RoverMixer, RoverProcessorResult> _gamepadProcessor;
        private int _pollerThreadSleepTime = 50; // default
        private ILogger<ucRoverGamepad> _logger;
        private int _gamepadIndex;

        public ucRoverGamepad()
        {
            InitializeComponent();
            SetStateStopped();
            InitializeIndexComboBox();
        }

        private void InitializeIndexComboBox()
        {
            cbGamepadIndex.DropDownStyle = ComboBoxStyle.DropDownList; // Disable free-text input
            cbGamepadIndex.DataSource = Enumerable.Range(0, 4).ToList();
            cbGamepadIndex.SelectedIndex = 0;
            cbGamepadIndex.SelectedIndexChanged += CbGamepadIndex_SelectedIndexChanged;
        }

        private void CbGamepadIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            _gamepadIndex = int.Parse(((ComboBox) sender).Text);
            btnStop_Click(this, EventArgs.Empty);
            btnStart_Click(this, EventArgs.Empty);
        }

        public void Setup(ILifetimeScope autofac)
        {
            Autofac = autofac;
            _pollerThreadSleepTime = Autofac.Resolve<IConfiguration>().GetValue<int>("gamepadUpdateFrequency");
            _gamepadProcessor = Autofac.Resolve<IGamepadProcessor<RoverMixer, RoverProcessorResult>>();
            _logger = Autofac.Resolve<ILogger<ucRoverGamepad>>();
            lblAcc.Text = string.Empty;
            lblDir.Text = string.Empty;
            pbAcc.Minimum = 0;
            pbAcc.Maximum = 100;
            pbDir.Minimum = 0;
            pbDir.Maximum = 100;
        }

        private void _poller_GamepadStateChanged(object sender, GamepadEventArgs e)
        {
            var processed = _gamepadProcessor.Process(e.Gamepad);
            // TODO eventBus publish
            UpdateResultWidgets(processed);
        }

        private void UpdateResultWidgets(RoverProcessorResult result)
        {
            Invoke(new Action(() =>
            {
                lblAcc.Text = result.Acceleration.ToString("0.##");
                lblDir.Text = result.Direction.ToString("0.##");
                pbAcc.SetProgressNoAnimation((int)(result.Acceleration / 2.55d)); // TODO fix range after implementing gamepad mixer
                pbDir.SetProgressNoAnimation((int)(result.Direction / 2.55d));
            }));
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (_isStarted)
            {
                _logger.LogWarning("Already started!");
                return;
            }
            
            _poller = new GamepadPoller(_gamepadIndex, _pollerThreadSleepTime);
            _poller.GamepadStateChanged += _poller_GamepadStateChanged;
            _poller.StartPolling();

            _logger.LogInformation($"Rover gamepad started with index: {_gamepadIndex}");
            SetStateStarted();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (_isStarted == false) return;

            _poller.StopPolling();
            _poller.GamepadStateChanged -= _poller_GamepadStateChanged;

            _logger.LogInformation("Rover gamepad stopped");
            SetStateStopped();
        }

        private void SetStateStarted()
        {
            lblState.Text = "Started";
            lblState.ForeColor = Color.Green;

            _isStarted = true;
        }

        private void SetStateStopped()
        {
            lblState.Text = "Stopped";
            lblState.ForeColor = Color.Red;

            lblAcc.Text = string.Empty;
            lblDir.Text = string.Empty;
            pbAcc.SetProgressNoAnimation(0);
            pbDir.SetProgressNoAnimation(0);

            _isStarted = false;
        }
    }
}
