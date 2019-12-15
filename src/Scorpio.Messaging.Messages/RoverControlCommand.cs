using Newtonsoft.Json;
using Scorpio.Messaging.Abstractions;

namespace Scorpio.Messaging.Messages
{
    public class RoverControlCommand : IntegrationEvent
    {
        [JsonProperty("dir")]
        public double Dir { get; }

        [JsonProperty("acc")]
        public double Acc { get; }

        public RoverControlCommand(double dir, double acc)
        {
            Dir = dir;
            Acc = acc;
        }
    }
}
