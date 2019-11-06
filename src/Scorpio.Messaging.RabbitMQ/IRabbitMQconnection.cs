using RabbitMQ.Client;
using System;

namespace Scorpio.Messaging.RabbitMQ
{
    public interface IRabbitMqConnection : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }
}
