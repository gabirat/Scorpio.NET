using Newtonsoft.Json;
using Scorpio.Messaging.Abstractions;
using System;

namespace Scorpio.Api.Events
{
    public class SaveSensorDataEvent : IntegrationEvent
    {
        [JsonProperty("value")]
        public double Value { get; set; }

        [JsonProperty("sensorKey")]
        public string SensorKey { get; set; }

        [JsonProperty("time")]
        public DateTime? Time { get; set; }
    }
}
