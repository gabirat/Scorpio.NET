using Newtonsoft.Json;
using System;

namespace Scorpio.Messaging.Abstractions
{
    public abstract class IntegrationEvent : IIntegrationEvent
    {
        protected IntegrationEvent()
        {
            CreationDate = DateTime.UtcNow;
        }

        [JsonConstructor]
        protected IntegrationEvent(Guid id, DateTime createDate)
        {
            CreationDate = createDate;
        }

        [JsonProperty("creationDate")]
        public DateTime? CreationDate { get; private set; }
    }
}
