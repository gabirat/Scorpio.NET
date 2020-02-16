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
            var host = "localhost";
            var factory = new ConnectionFactory() { HostName = host, UserName = "guest", Password = "guest", VirtualHost = "/" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.ExchangeDeclare("scorpio.direct", ExchangeType.Direct);

            // my queue
            channel.QueueDeclare("scorpio.stub", true, false, false, null);
            channel.QueueBind("scorpio.stub", "scorpio.direct", "RoverControlCommand");
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queue: "scorpio.stub", autoAck: true, consumer: consumer);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var msgType = ea.RoutingKey;
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine("{0}   Received {1} {2}", DateTime.UtcNow, msgType, message);
                //channel.BasicAck(ea.DeliveryTag, false);
            };


            Console.ReadKey();
        }
    }
}