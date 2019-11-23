using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Scorpio.Messaging.Stub
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", UserName="guest", Password="guest", VirtualHost="/" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.ExchangeDeclare("scorpio.direct", ExchangeType.Direct);
            channel.ExchangeDeclare("scorpio.broadcast", ExchangeType.Fanout);

            // my queue
            channel.QueueDeclare(queue: "scorpio.csharpqueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueBind("scorpio.csharpqueue", "scorpio.broadcast", "UpdateRoverPositionEvent");
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var msgType = ea.RoutingKey;
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine("[x] Received {0} {1}", msgType, message);
            };

            channel.BasicConsume(queue: "scorpio.csharpqueue", autoAck: true, consumer: consumer);

            // sending
            while (true)
            {
                using var responseConnection = factory.CreateConnection();
                using var responseChannel = responseConnection.CreateModel();
                var props = responseChannel.CreateBasicProperties();
                props.Expiration = "5000";
                responseChannel.BasicPublish("scorpio.direct", "Event", props, Encoding.UTF8.GetBytes("body"));
                Console.WriteLine("Message published to exchange scorpio.direct, routingKey = 'Event'");
                Thread.Sleep(500);
            }
        }
    }
}
