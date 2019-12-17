using Newtonsoft.Json;
using Scorpio.Messaging.Abstractions;

namespace Scorpio.Messaging.Messages
{
    public class UbiquitiDataReceivedEvent : IntegrationEvent
    {
        [JsonProperty("data")]
        public object Data { get; }

        public UbiquitiDataReceivedEvent(object data)
        {
            Data = data;
        }
    }
}
