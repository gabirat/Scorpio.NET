using System.Threading;

namespace Scorpio.Messaging.Abstractions
{
    public interface IEventBus
    {
        void Publish(IntegrationEvent @event);

        void Subscribe<TEvent, THandler>(string keyOverride = null) 
            where TEvent : IIntegrationEvent 
            where THandler : IIntegrationEventHandler<TEvent>;

        void Unsubscribe<TEvent, THandler>(string keyOverride = null) 
            where THandler : IIntegrationEventHandler<TEvent> 
            where TEvent : IIntegrationEvent;
    }
}
