using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using Newtonsoft.Json;
using System.Threading;

namespace Scorpio.Messaging.Stub
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = "80.68.231.116";
            var factory = new ConnectionFactory() { HostName = host, UserName = "guest", Password = "jebacdarka", VirtualHost = "/" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.ExchangeDeclare("scorpio.direct", ExchangeType.Direct);

            // my queue
            channel.QueueDeclare("scorpio.mati", true, false, false, null);
            //channel.QueueDeclare(queue: "scorpiox.mati", durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueBind("scorpio.mati", "scorpio.direct", "UpdateRoverPositionEvent");
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queue: "scorpio.mati", autoAck: true, consumer: consumer);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var msgType = ea.RoutingKey;
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine("{0}   Received {1} {2}", DateTime.UtcNow, msgType, message);
                Thread.Sleep(2500);
                //channel.BasicAck(ea.DeliveryTag, false);
            };


            Console.ReadKey();
        }
    }
}