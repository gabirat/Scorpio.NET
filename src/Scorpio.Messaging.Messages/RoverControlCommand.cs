using Newtonsoft.Json;
using Scorpio.Messaging.Abstractions;

namespace Scorpio.Messaging.Messages
{
    public class RoverControlCommand : IntegrationEvent
    {
        [JsonProperty("dir")]
        public float Dir { get; }

        [JsonProperty("acc")]
        public float Acc { get; }

        public RoverControlCommand(float dir, float acc)
        {
            Dir = dir;
            Acc = acc;
        }

        public RoverControlCommand()
        {
        }
    }
}
