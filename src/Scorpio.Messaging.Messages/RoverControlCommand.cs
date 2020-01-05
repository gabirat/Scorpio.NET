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

        [JsonProperty("rot")]
        public float Rot { get; set; }

        [JsonProperty("doRot")]
        public bool DoRot { get; set; }

        public RoverControlCommand(float dir, float acc, float rot, bool doRotation)
        {
            Dir = dir;
            Acc = acc;
            Rot = rot;
            DoRot = doRotation;
        }
    }
}
