using Newtonsoft.Json;
using Scorpio.Messaging.Abstractions;

namespace Scorpio.Messaging.Messages
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
