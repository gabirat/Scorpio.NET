using Newtonsoft.Json;
using Scorpio.Messaging.Abstractions;

namespace Scorpio.Api.Events
{
    public class RoverControlEvent : IntegrationEvent
    {
        [JsonProperty("dir")]
        public double Dir { get; }

        [JsonProperty("acc")]
        public double Acc { get; }

        public RoverControlEvent(double dir, double acc)
        {
            Dir = dir;
            Acc = acc;
        }
    }
}
