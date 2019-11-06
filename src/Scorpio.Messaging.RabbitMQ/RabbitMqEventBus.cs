using System;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Scorpio.Messaging.Abstractions;

namespace Scorpio.Messaging.RabbitMQ
{
    public class RabbitMqEventBus : IEventBus, IDisposable
    {
        private const string ExchangeName = "MainEventBus";
        private readonly IEventBusSubscriptionManager _subsManager;
        private readonly ILogger<RabbitMqEventBus> _logger;
        private readonly ILifetimeScope _autofac;
        private IModel _consumerChannel;
        private readonly IRabbitMqConnection _persistentConnection;
        private readonly string _queueName;

        public RabbitMqEventBus(IRabbitMqConnection persistentConnection, ILogger<RabbitMqEventBus> logger,
            ILifetimeScope autofac, IEventBusSubscriptionManager subsManager, string queueName)
        {
            _queueName = queueName ?? throw new ArgumentNullException(nameof(queueName));
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subsManager = subsManager ?? new GenericEventBusSubscriptionManager();
            _consumerChannel = CreateConsumerChannel();
            _autofac = autofac;
        }

        public void Publish(IntegrationEvent @event)
        {
            var routingKey = @event.GetType().Name;

            //var factory = new ConnectionFactory
            //{
            //    HostName = "localhost", 
            //    Port = 5672, 
            //    UserName = "guest", 
            //    Password = "guest", 
            //    VirtualHost = "/"
            //};

            using (var channel = _persistentConnection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: ExchangeName, type: "direct");
                var props = channel.CreateBasicProperties();
                props.DeliveryMode = 2; // persistent
                props.Expiration = "1000"; // ms TTL

                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: ExchangeName,
                    routingKey: routingKey,
                    basicProperties: props,
                    body: body);
            }


            //using (var connection = factory.CreateConnection())
            //using (var channel = connection.CreateModel())
            //{
            //    channel.ExchangeDeclare(exchange: ExchangeName, type: "direct");
            //    var props = channel.CreateBasicProperties();
            //    props.DeliveryMode = 2; // persistent
            //    props.Expiration = "1000"; // ms TTL

            //    var message = JsonConvert.SerializeObject(@event);
            //    var body = Encoding.UTF8.GetBytes(message);
            //    channel.BasicPublish(exchange: ExchangeName,
            //        routingKey: routingKey,
            //        basicProperties: props,
            //        body: body);
            //}
        }

        public void Subscribe<TEvent, THandler>() where TEvent : IIntegrationEvent where THandler : IIntegrationEventHandler<TEvent>
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
                    channel.QueueBind(queue: _queueName,
                        exchange: ExchangeName,
                        routingKey: eventName);
                }
            }
            _subsManager.AddSubscription<TEvent, THandler>();
        }

        public void Unsubscribe<TEvent, THandler>() where TEvent : IIntegrationEvent where THandler : IIntegrationEventHandler<TEvent>
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
                _persistentConnection.TryConnect();

            var channel = _persistentConnection.CreateModel();
            channel.ExchangeDeclare(ExchangeName, "direct");
            channel.QueueDeclare(_queueName, true, false, false, null);

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
            if (_subsManager.HasSubscriptionsForEvent(eventName))
            {
                using (var scope = _autofac.BeginLifetimeScope(ExchangeName))
                {
                    var subscriptions = _subsManager.GetHandlersForEvent(eventName);
                    foreach (var subscription in subscriptions)
                    {
                        var handler = scope.ResolveOptional(subscription.HandlerType);
                        if (handler is null) continue;
                        var eventType = _subsManager.GetEventTypeByName(eventName);
                        var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                        await (Task)concreteType.GetMethod("Handle")
                            .Invoke(handler, new object[] { integrationEvent });
                    }
                }
            }
        }
    }
}
