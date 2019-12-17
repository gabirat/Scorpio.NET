using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Scorpio.Instrumentation.Ubiquiti;
using Scorpio.Messaging.Abstractions;
using Scorpio.Messaging.Messages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Scorpio.Api.HostedServices
{
    public class UbiquitiPollerHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<UbiquitiPollerHostedService> _logger;
        private readonly UbiquitiStatsProvider _ubiProvider;
        private readonly IEventBus _eventBus;
        private Timer _timer;

        public UbiquitiPollerHostedService(ILogger<UbiquitiPollerHostedService> 
            logger, UbiquitiStatsProvider provider,
            IEventBus eventBus)
        {
            _logger = logger;
            _ubiProvider = provider;
            _eventBus = eventBus;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("UbiquitiPollerHostedService running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            try
            {
                var stats = await _ubiProvider.GetStatsAsync();

                if (stats != null && stats.Count > 0)
                {
                    _eventBus?.Publish(new UbiquitiDataReceivedEvent(stats));
                }
            }
            catch (Exception ex)
            {
                const string msg = "Could not connect to SNMP host - make sure it is in the same network";
                _logger.LogError(msg, ex.Message, ex.Message, ex.ToString());
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("UbiquitiPollerHostedService is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
