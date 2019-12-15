using System;
using System.Drawing;
using System.Windows.Forms;
using Autofac;
using Scorpio.GUI.Streaming;

namespace Scorpio.GUI.Controls
{
    public partial class ucStreamControl : UserControl
    {
        private ILifetimeScope _autofac;
        public ILifetimeScope Autofac
        {
            get => _autofac ?? throw new ArgumentNullException("Autofac was not initialized in this control.");
            set => _autofac = value;
        }

        private string _cameraId;
        public string CameraId
        {
            get => _cameraId ?? throw new ArgumentNullException("CameraId was not provided.");
            set => _cameraId = value;
        }

        private const string STARTED = "Started!";
        private const string STOPPED = "Stopped";

        private GStreamerLauncher _gStreamer;
        private GStreamerLauncher GStreamer => _gStreamer ?? (_gStreamer = _autofac.Resolve<GStreamerLauncher>());

        private CameraConfigModel _camConfig;
        private CameraConfigModel CamConfig => _camConfig ?? (_camConfig = _autofac.Resolve<CameraConfigModel>());

        public ucStreamControl()
        {
            InitializeComponent();
            lblState.Text = STOPPED;
            lblState.BackColor = Color.Red;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            var config = CamConfig.GetStreamById(CameraId);
            GStreamer.Launch(config.GstreamerArg);

            lblState.Text = STARTED;
            lblState.BackColor = Color.Green;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            var config = CamConfig.GetStreamById(CameraId);
            GStreamer.Stop(config.GstreamerArg);

            lblState.Text = STOPPED;
            lblState.BackColor = Color.Red;
        }
    }
}
