using Newtonsoft.Json;
using System;

namespace Scorpio.Messaging.Abstractions
{
    public abstract class IntegrationEvent : IIntegrationEvent
    {
        protected IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        [JsonConstructor]
        protected IntegrationEvent(Guid id, DateTime createDate)
        {
            Id = id;
            CreationDate = createDate;
        }

        [JsonProperty("id")]
        public Guid Id { get; private set; }

        [JsonProperty("creationDate")]
        public DateTime CreationDate { get; private set; }
    }
}
