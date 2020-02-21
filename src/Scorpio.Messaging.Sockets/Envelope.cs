using Newtonsoft.Json;
using Scorpio.Messaging.Abstractions;
using System;
using System.Net;
using System.Text;

namespace Scorpio.Messaging.Sockets
{
    /// <summary>
    /// Represents wrapper around network message (integration event)
    /// </summary>
    public class Envelope
    {
        /// <summary>
        /// Message unique key
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; set; }

        /// <summary>
        /// Payload object
        /// </summary>
        [JsonProperty("data")]
        public object Data { get; set; }

        /// <summary>
        /// Builds an envelope from integration event.
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public static Envelope Build(IntegrationEvent @event)
        {
            var key = string.IsNullOrWhiteSpace(@event.KeyOverride) ? @event.GetType().Name : @event.KeyOverride;

            return new Envelope
            {
                Data = @event,
                Key = key
            };
        }

        /// <summary>
        /// Serialize envelope to byte array
        /// </summary>
        /// <returns></returns>
        public byte[] Serialize() => Serialize(this);

        /// <summary>
        /// Serialize envelope to byte array
        /// </summary>
        /// <param name="envelope"></param>
        /// <returns></returns>
        public byte[] Serialize(Envelope envelope)
        {
            var message = JsonConvert.SerializeObject(envelope);
            var payload = Encoding.UTF8.GetBytes(message);
            //var header = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(body.Length));
            var header = BitConverter.GetBytes(payload.Length);
            var packet = new byte[payload.Length + sizeof(int)];

            // The packet is:
            // 0x01 0x00 0x00 0x00  <- header, indicating message length of 1
            // 0xaa <- example payload (actually invalid, needs to be valid JSON)
            Buffer.BlockCopy(header, 0, packet, 0, sizeof(int));
            Buffer.BlockCopy(payload, 0, packet, sizeof(int), payload.Length);

            return packet;
        }

        /// <summary>
        /// Deserialize byte array to envelope
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
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
