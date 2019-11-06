using Scorpio.Messaging.Abstractions;

namespace Scorpio.Api.Events
{
    public class UpdateRoverPositionEvent : IntegrationEvent
    {
        public string PosX { get; }
        public string PosY { get; }

        public UpdateRoverPositionEvent(string posX, string posY)
        {
            PosX = posX;
            PosY = posY;
        }
    }
}
