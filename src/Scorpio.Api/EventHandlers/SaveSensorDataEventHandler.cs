using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Scorpio.Api.DataAccess;
using Scorpio.Api.Events;
using Scorpio.Api.Hubs;
using Scorpio.Api.Models;
using Scorpio.Messaging.Abstractions;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Scorpio.Api.Validation;

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

            SensorDataValidatorExecutor.Execute(entity, true);

            var created = await _sensorDataRepository.CreateAsync(entity);

            var data = JsonConvert.SerializeObject(created);

            // Notify UI via SignalR (uses 'sensor' topic)
            await _hubContext.Clients.All.SendAsync(Constants.Topics.Sensor, data);
        }
    }
}
