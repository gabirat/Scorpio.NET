using Newtonsoft.Json;
using Scorpio.Messaging.Abstractions;
using System;
using System.Net;
using System.Text;

namespace Scorpio.Messaging.Sockets
{
    public class Envelope
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }

        public static byte[] Build(IntegrationEvent @event)
        {
            var key = string.IsNullOrWhiteSpace(@event.KeyOverride) ? @event.GetType().Name : @event.KeyOverride;

            var enveloped = new Envelope
            {
                Data = @event,
                Key = key
            };

            var message = JsonConvert.SerializeObject(enveloped);
            var body = Encoding.UTF8.GetBytes(message);
            var header = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(body.Length));
            var packet = new byte[body.Length + sizeof(int)];

            // The packet is:
            // 0x01 0x00 0x00 0x00  <- header, indicating message length of 1
            // 0xaa <- example payload (actually invalid, needs to be valid JSON)
            Buffer.BlockCopy(header, 0, packet, 0, sizeof(int));
            Buffer.BlockCopy(body, 0, packet, sizeof(int), body.Length);

            return packet;
        }

        public static Envelope Deserialize(byte[] data)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            var json = Encoding.UTF8.GetString(data);
            var envelope = JsonConvert.DeserializeObject<Envelope>(json);
            return envelope;
        }
    }
}
