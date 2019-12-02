using Newtonsoft.Json;
using Scorpio.Messaging.Abstractions;
using System;
using System.Collections.Generic;

namespace Scorpio.Api.Events
{
    public class SaveManySensorDataEvent : IntegrationEvent
    {
        [JsonProperty("values")]
        public IEnumerable<SensorDataEventDto> Values { get; set; }
    }

    public class SensorDataEventDto
    {
        [JsonProperty("value")]
        public double Value { get; set; }

        [JsonProperty("sensorKey")]
        public string SensorKey { get; set; }

        [JsonProperty("time")]
        public DateTime? Time { get; set; }
    }
}
