using System.Threading.Tasks;

namespace Scorpio.Messaging.Abstractions
{
    public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler where TIntegrationEvent : IIntegrationEvent
    {
        Task Handle(TIntegrationEvent @event);
    }

    public interface IIntegrationEventHandler
    {
    }
}
