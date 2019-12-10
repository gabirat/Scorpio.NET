using Microsoft.Extensions.Logging;
using System;
using System.Timers;

namespace Scorpio.GUI
{
    public class Sender : IDisposable
    {
        public int Period { get; set; }
        private Timer _timer;
        private readonly ILogger<Sender> _logger;

        public Sender(ILogger<Sender> logger)
        {
            _logger = logger;
        }

        public void Start(double interval)
        {
            _timer = new Timer(interval);
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
        }

        public void Stop() => _timer?.Stop();

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _logger.LogDebug("TIMER LogDebug");
            _logger.LogInformation("TIMER LogInformation");
            _logger.LogWarning("TIMER LogWarning");
            _logger.LogError("TIMER LogError");            
            _logger.LogCritical("TIMER LogCritical");
        }

        public void Dispose() => _timer?.Dispose();
    }
}
