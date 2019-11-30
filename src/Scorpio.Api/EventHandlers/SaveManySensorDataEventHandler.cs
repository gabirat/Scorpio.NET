using Microsoft.AspNetCore.SignalR;
using Scorpio.Api.DataAccess;
using Scorpio.Api.Events;
using Scorpio.Api.Hubs;
using Scorpio.Api.Models;
using Scorpio.Messaging.Abstractions;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Scorpio.Api.EventHandlers
{
    public class SaveManySensorDataEventHandler : IIntegrationEventHandler<SaveManySensorDataEvent>
    {
        private readonly ISensorDataRepository _sensorDataRepository;
        private readonly IHubContext<MainHub> _hubContext;

        public SaveManySensorDataEventHandler(ISensorDataRepository sensorDataRepository, IHubContext<MainHub> hubContext)
        {
            _sensorDataRepository = sensorDataRepository;
            _hubContext = hubContext;
        }

        public async Task Handle(SaveManySensorDataEvent @event)
        {
            foreach (var value in @event.Values)
            {
                // Validate
                if (string.IsNullOrEmpty(value.SensorKey) || !double.TryParse(value.Value.ToString(), out _))
                {
                    continue;
                }

                // Save in DB
                var entity = new SensorData
                {
                    SensorKey = value.SensorKey,
                    Value = value.Value.ToString(CultureInfo.InvariantCulture),
                    TimeStamp = value.Time ?? DateTime.UtcNow
                };

                var created = await _sensorDataRepository.CreateAsync(entity);

                // Notify UI via SignalR (uses 'sensor' topic)
                await _hubContext.Clients.All.SendAsync("sensor", created);
            }
        }
    }
}
