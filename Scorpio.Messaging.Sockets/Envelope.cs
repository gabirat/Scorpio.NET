using Newtonsoft.Json;
using Scorpio.Messaging.Abstractions;
using System.Text;

namespace Scorpio.Messaging.Sockets
{
    public class Envelope
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("d")]
        public object Data { get; set; }


        public static byte[] Build(IntegrationEvent @event)
        {
            var key = @event.GetType().Name;
            var enveloped = new Envelope
            {
                Data = @event,
                Key = key
            };

            var message = JsonConvert.SerializeObject(enveloped);
            var body = Encoding.UTF8.GetBytes(message);

            return body;
        }
    }
}
