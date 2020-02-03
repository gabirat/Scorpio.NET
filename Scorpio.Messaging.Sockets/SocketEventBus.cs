using Scorpio.Messaging.Abstractions;
using System;
using System.IO;
using System.Text;
using Autofac;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
            _socketClient = socketClient ?? throw new ArgumentNullException(nameof(socketClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _busSubscriptionManager = busSubscriptionManager ?? throw new ArgumentNullException(nameof(busSubscriptionManager));
            _autofac = autofac ?? throw new ArgumentNullException(nameof(autofac));
            _socketClient.TryConnect();
        }

        public void Publish(IntegrationEvent @event)
        {
            var message = Envelope.Build(@event);
            try
            {
                _socketClient.Stream.Write(message, 0, message.Length);
            }
            catch (IOException ex)
            {
                _logger.LogError("Lost connection while writing message", ex.Message, ex);
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
            _busSubscriptionManager.RemoveSubscription<TEvent, THandler>();
        }
    }
}
