namespace Scorpio.Messaging.Abstractions
{
    public interface IIntegrationEvent
    {
        string KeyOverride { get; }
        string QueueName { get; }
    }
}