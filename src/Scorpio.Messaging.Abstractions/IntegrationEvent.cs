using Newtonsoft.Json;

namespace Scorpio.Messaging.Abstractions
{
    /// <summary>
    /// Base class for creating bus messages (both events and commands).
    /// If needed, some common concepts might be introduced here (correlationId, creationTime and so on).
    /// </summary>
    public abstract class IntegrationEvent : IIntegrationEvent
    {
        [JsonIgnore]
        public string KeyOverride { get; protected set; }

        [JsonIgnore]
        public string QueueName { get; protected set; }
    }
}
