using Scorpio.Api.DataAccess;
using Scorpio.Api.Events;
using Scorpio.Api.Models;
using Scorpio.Messaging.Abstractions;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Scorpio.Api.Hubs;

namespace Scorpio.Api.EventHandlers
{
    public class SaveSensorDataEventHandler : IIntegrationEventHandler<SaveSensorDataEvent>
    {
        private readonly ISensorDataRepository _sensorDataRepository;
        private readonly IHubContext<MainHub> _hubContext;

        public SaveSensorDataEventHandler(ISensorDataRepository sensorDataRepository, IHubContext<MainHub> hubContext)
        {
            _sensorDataRepository = sensorDataRepository;
            _hubContext = hubContext;
        }

        public async Task Handle(SaveSensorDataEvent @event)
        {
            // Validate
            if (string.IsNullOrEmpty(@event.SensorKey) || !double.TryParse(@event.Value.ToString(), out _))
            {
                return;
            }

            // Save in DB
            var entity = new SensorData
            {
                SensorKey = @event.SensorKey,
                Value = @event.Value.ToString(CultureInfo.InvariantCulture),
                TimeStamp = @event.Time ?? DateTime.UtcNow
            };

            var created = await _sensorDataRepository.CreateAsync(entity);

            // Notify UI via SignalR (uses 'sensor' topic)
            await _hubContext.Clients.All.SendAsync("sensor", created);
        }
    }
}
