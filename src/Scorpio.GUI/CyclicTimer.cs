using Microsoft.Extensions.Logging;
using System;
using System.Timers;

namespace Scorpio.GUI
{
    public class CyclicTimer : IDisposable
    {
        public Action ElapsedAction;

        private Timer _timer;
        private readonly ILogger<CyclicTimer> _logger;

        public CyclicTimer(ILogger<CyclicTimer> logger)
        {
            _logger = logger;
        }

        public void Start(double interval)
        {
            _timer = new Timer(interval);
            _timer.Elapsed += (_, __) => ElapsedAction?.Invoke();
            _timer.Start();
            _logger.LogInformation($"Sender has been started with inverval: {interval} [ms]");
        }

        public void Stop()
        {
            _timer?.Stop();
            _logger.LogInformation("Sender has been stopped");
        }

        public void Dispose() => _timer?.Dispose();
    }
}
