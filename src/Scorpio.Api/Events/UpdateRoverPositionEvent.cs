using Newtonsoft.Json;
using Scorpio.Messaging.Abstractions;

namespace Scorpio.Api.Events
{
    public class UpdateRoverPositionEvent : IntegrationEvent
    {
        [JsonProperty("posX")]
        public string PosX { get; }

        [JsonProperty("posY")]
        public string PosY { get; }

        public UpdateRoverPositionEvent(string posX, string posY)
        {
            PosX = posX;
            PosY = posY;
        }

        public UpdateRoverPositionEvent()
        {
        }
    }
}
