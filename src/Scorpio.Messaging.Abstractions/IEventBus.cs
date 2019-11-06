namespace Scorpio.Messaging.Abstractions
{
    public interface IEventBus
    {
        void Publish(IntegrationEvent @event);
        void Subscribe<TEvent, THandler>() where TEvent : IIntegrationEvent where THandler : IIntegrationEventHandler<TEvent>;
        void Unsubscribe<TEvent, THandler>() where THandler : IIntegrationEventHandler<TEvent> where TEvent : IIntegrationEvent;
    }
}
