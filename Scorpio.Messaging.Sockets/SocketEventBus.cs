using Autofac;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Scorpio.Messaging.Abstractions;
using Scorpio.Messaging.Sockets.Workers;
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
        private readonly object _sendSyncLock;
        private NetworkWorkersFacade _workersFacade;

        public SocketEventBus(ISocketClient socketClient,
            ILogger<SocketEventBus> logger,
            IEventBusSubscriptionManager busSubscriptionManager,
            ILifetimeScope autofac)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _busSubscriptionManager = busSubscriptionManager ?? throw new ArgumentNullException(nameof(busSubscriptionManager));
            _autofac = autofac ?? throw new ArgumentNullException(nameof(autofac));
            _sendSyncLock = new object();

            _socketClient = socketClient ?? throw new ArgumentNullException(nameof(socketClient));
            _socketClient.Connected += (s, e) => CreateWorkersFacade();
            _socketClient.Disconnected += (s, e) => DestroyWorkerFacade();
            _socketClient.TryConnect();
        }
        
        private void CreateWorkersFacade()
        {
            _logger.LogInformation("Creating workers facade...");
            _workersFacade = new NetworkWorkersFacade(_autofac);
            _workersFacade.NetworkWorkerFaulted += _workersFacade_NetworkWorkerFaulted;
            _workersFacade.PacketReceived += _workersFacade_PacketReceived;
            _workersFacade.NetworkStream = _socketClient?.Stream;
            _workersFacade.Start();
        }

        private void DestroyWorkerFacade()
        {
            _logger.LogInformation("Destroying workers facade...");
            _workersFacade.NetworkWorkerFaulted -= _workersFacade_NetworkWorkerFaulted;
            _workersFacade.PacketReceived -= _workersFacade_PacketReceived;
            _workersFacade.Stop();
            _workersFacade = null;
        }

        private void _workersFacade_NetworkWorkerFaulted(object sender, FaultExceptionEventArgs e)
        {
            var ex = e.GetException();
            _logger.LogError($"{sender.GetType().FullName} faulted: " + ex?.Message, ex);

            lock (_sendSyncLock)
            {
                _socketClient.Disconnect();
                _socketClient.TryConnect();
            }
        }

        private async void _workersFacade_PacketReceived(object sender, PacketReceivedEventArgs e)
        {
            try
            {
                var envelope = Envelope.Deserialize(e.Packet);
                await ProcessEvent(envelope);
            }
            catch (JsonSerializationException)
            {
                _logger.LogWarning("Received message, but cannot deserialize (invalid protocol)");
            }
            catch (JsonReaderException)
            {
                _logger.LogWarning("Received message, but cannot deserialize (invalid protocol)");
            }
        }
        
        private async Task ProcessEvent(Envelope envelope)
        {
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

        public void Publish(IntegrationEvent @event)
        {
            try
            {
                lock (_sendSyncLock)
                {
                    if (!_socketClient.IsConnected)
                        _socketClient.TryConnect();

                    if (_socketClient.Stream != null
                        && _socketClient.Stream.CanWrite
                        && _workersFacade.SenderStatus == WorkerStatus.Running)
                    {
                        byte[] message = Envelope.Build(@event);
                        _workersFacade.Enqueue(message);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Lost connection while writing message: " + ex.Message, ex);
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
