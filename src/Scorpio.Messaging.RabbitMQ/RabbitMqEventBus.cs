using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Scorpio.Messaging.Abstractions;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Scorpio.Messaging.RabbitMQ
{
    public class RabbitMqEventBus : IEventBus, IDisposable
    {
        private readonly string _exchangeName;
        private readonly IEventBusSubscriptionManager _subsManager;
        private readonly ILogger<RabbitMqEventBus> _logger;
        private readonly ILifetimeScope _autofac;
        private readonly IConfiguration _config;
        private readonly IRabbitMqConnection _persistentConnection;
        private readonly string _queueName;
        private IModel _consumerChannel;

        public RabbitMqEventBus(IRabbitMqConnection persistentConnection, ILogger<RabbitMqEventBus> logger,
            ILifetimeScope autofac, IEventBusSubscriptionManager subsManager, IConfiguration config)
        {
            _queueName = config["RabbitMq:myQueueName"] ?? throw new ArgumentNullException(nameof(_queueName));
            _exchangeName = config["RabbitMq:exchangeName"] ?? throw new ArgumentNullException(nameof(_exchangeName));
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _config = config;
            _autofac = autofac;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subsManager = subsManager ?? new GenericEventBusSubscriptionManager();
            _consumerChannel = CreateConsumerChannel();
        }

        public void Publish(IntegrationEvent @event)
        {
            var routingKey = @event.GetType().Name;

            using (var channel = _persistentConnection.CreateModel())
            {
                var props = ConfigureChannel(channel);
                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: _exchangeName, routingKey: routingKey, basicProperties: props, body: body);
            }
        }

        private IBasicProperties ConfigureChannel(IModel channel)
        {
            var expiration = _config["RabbitMq:messageTTL"] ?? throw new ArgumentException("RabbitMq:messageTTL");
            var props = channel.CreateBasicProperties();
            props.DeliveryMode = 1; // non persistent
            props.Expiration = expiration; // ms TTL
            props.ContentType = "application/json";
            props.ContentEncoding = "UTF8";
            return props;
        }

        public void Subscribe<TEvent, THandler>()
            where TEvent : IIntegrationEvent
            where THandler : IIntegrationEventHandler<TEvent>
        {
            var eventName = _subsManager.GetEventKey<TEvent>();

            if (!_subsManager.HasSubscriptionsForEvent(eventName))
            {
                if (!_persistentConnection.IsConnected)
                {
                    _persistentConnection.TryConnect();
                }

                using (var channel = _persistentConnection.CreateModel())
                {
                    channel.QueueBind(queue: _queueName, exchange: _exchangeName, routingKey: eventName);
                }

                var log = $"RabbitMQ bound routingKey: {eventName} to queue: {_queueName} using exchange {_exchangeName}";
                _logger.LogInformation(log);
            }
            _subsManager.AddSubscription<TEvent, THandler>();
        }

        public void Unsubscribe<TEvent, THandler>()
            where TEvent : IIntegrationEvent
            where THandler : IIntegrationEventHandler<TEvent>
        {
            _subsManager.RemoveSubscription<TEvent, THandler>();
        }

        public void Dispose()
        {
            _consumerChannel?.Dispose();
            _subsManager.Clear();
        }

        private IModel CreateConsumerChannel()
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var channel = _persistentConnection.CreateModel();
            channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct);
            channel.QueueDeclare(_queueName, false, false, false, null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += async (model, ea) =>
            {
                var eventName = ea.RoutingKey;
                var message = Encoding.UTF8.GetString(ea.Body);

                await ProcessEvent(eventName, message);

                channel.BasicAck(ea.DeliveryTag, false);
            };

            channel.BasicConsume(_queueName, false, consumer);

            channel.CallbackException += (sender, ea) =>
            {
                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
            };

            return channel;
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if (!_subsManager.HasSubscriptionsForEvent(eventName))
            {
                return;
            }

            using (var scope = _autofac.BeginLifetimeScope(_exchangeName))
            {
                var subscriptions = _subsManager.GetHandlersForEvent(eventName);
                foreach (var subscription in subscriptions)
                {
                    var handler = scope.ResolveOptional(subscription.HandlerType);
                    if (handler is null) continue;

                    var eventType = _subsManager.GetEventTypeByName(eventName);
                    var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                    await (Task)concreteType
                        .GetMethod(nameof(IIntegrationEventHandler<IIntegrationEvent>.Handle))
                        .Invoke(handler, new[] { integrationEvent });
                }
            }
        }
    }
}
