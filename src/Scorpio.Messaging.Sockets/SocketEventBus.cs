using Autofac;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Scorpio.Messaging.Abstractions;
using System;
using System.Threading.Tasks;

namespace Scorpio.Messaging.Sockets
{
    public class SocketEventBus : IEventBus
    {
        private readonly ISocketClient _socketClient;
        private readonly ILogger<SocketEventBus> _logger;
        private readonly IEventBusSubscriptionManager _busSubscriptionManager;
        private readonly ILifetimeScope _autofac;

        public SocketEventBus(ISocketClient socketClient,
            ILogger<SocketEventBus> logger,
            IEventBusSubscriptionManager busSubscriptionManager,
            ILifetimeScope autofac)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _busSubscriptionManager = busSubscriptionManager ?? throw new ArgumentNullException(nameof(busSubscriptionManager));
            _autofac = autofac ?? throw new ArgumentNullException(nameof(autofac));

            _socketClient = socketClient ?? throw new ArgumentNullException(nameof(socketClient));
            _socketClient.MessageReceived += async (s, e) => await ProcessEvent(e?.Envelope);
            _socketClient.TryConnect();
        }

        public void Publish(IntegrationEvent @event)
        {
            try
            {
                _socketClient.Enqueue(@event);
            }
            catch (Exception ex)
            {
                _logger.LogError("Lost connection while writing message: " + ex.Message, ex);
            }
        }

        private async Task ProcessEvent(Envelope envelope)
        {
            if (envelope is null)
                return;
            
            if (!_busSubscriptionManager.HasSubscriptionsForEvent(envelope.Key))
                return;

            using (var scope = _autofac.BeginLifetimeScope())
            {
                var subscriptions = _busSubscriptionManager.GetHandlersForEvent(envelope.Key);
                foreach (var subscription in subscriptions)
                {
                    var handler = scope.ResolveOptional(subscription.HandlerType);
                    if (handler is null) continue;

                    var eventType = _busSubscriptionManager.GetEventTypeByName(envelope.Key);
                    var integrationEvent = JsonConvert.DeserializeObject(envelope.Data?.ToString(), eventType);
                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                    
                    // ReSharper disable once PossibleNullReferenceException
                    await (Task)concreteType
                        .GetMethod(nameof(IIntegrationEventHandler<IIntegrationEvent>.Handle))
                        ?.Invoke(handler, new[] { integrationEvent });
                }
            }
        }

        public void Subscribe<TEvent, THandler>()
            where TEvent : IIntegrationEvent
            where THandler : IIntegrationEventHandler<TEvent>
        {
            var eventName = _busSubscriptionManager.GetEventKey<TEvent>();
            _logger.LogInformation($"Socket subscription manager: subscribed to: {eventName}");
            _busSubscriptionManager.AddSubscription<TEvent, THandler>();
        }

        public void Unsubscribe<TEvent, THandler>()
            where TEvent : IIntegrationEvent
            where THandler : IIntegrationEventHandler<TEvent>
        {
            var eventName = _busSubscriptionManager.GetEventKey<TEvent>();
            _logger.LogInformation($"Socket subscription manager: unsubscribed from: {eventName}");
            _busSubscriptionManager.RemoveSubscription<TEvent, THandler>();
        }
    }
}
