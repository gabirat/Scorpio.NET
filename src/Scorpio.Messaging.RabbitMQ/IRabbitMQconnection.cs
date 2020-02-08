using RabbitMQ.Client;
using System;
using System.Threading;

namespace Scorpio.Messaging.RabbitMQ
{
    public interface IRabbitMqConnection : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();
        bool TryConnect(CancellationToken cancellationToken);

        IModel CreateModel();

        event EventHandler<EventArgs> OnConnected;
        event EventHandler<EventArgs> OnDisconnected;
    }
}
