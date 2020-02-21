using System.Collections.Generic;
using Newtonsoft.Json;
using Scorpio.Messaging.Abstractions;

namespace Scorpio.Messaging.Messages
{
    public class AdvertiseCommand : IntegrationEvent
    {
        public override string KeyOverride => "advertise";

        [JsonProperty("keys")]
        public IEnumerable<string> Keys { get; set; }

        public AdvertiseCommand(IEnumerable<string> keys)
        {
            Keys = keys;
        }

        public AdvertiseCommand()
        {
        }
    }
}
