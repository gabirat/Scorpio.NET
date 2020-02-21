using Scorpio.Messaging.Abstractions;
using Scorpio.Messaging.Sockets.Workers;
using System;
using System.Threading;

namespace Scorpio.Messaging.Sockets
{
    /// <summary>
    /// Represents abstraction around the TCP socket
    /// </summary>
    public interface ISocketClient : IDisposable
    {
        /// <summary>
        /// Event when new message is received and parsed
        /// </summary>
        event EventHandler<MessageReceivedEventArgs> MessageReceived;

        /// <summary>
        /// Event when connection was established
        /// </summary>
        event EventHandler<EventArgs> Connected;

        /// <summary>
        /// Event when connection was lost
        /// </summary>
        event EventHandler<EventArgs> Disconnected;

        /// <summary>
        /// Enqueue new message to send
        /// </summary>
        /// <param name="event"></param>
        void Enqueue(IntegrationEvent @event);

        /// <summary>
        /// Returns true if connection is established
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Try connect to the host with default retry policy
        /// </summary>
        /// <returns></returns>
        void TryConnect();

        /// <summary>
        /// Try connect to the host with default retry policy
        /// </summary>
        /// <returns></returns>
        bool TryConnect(CancellationToken cancellationToken);

        /// <summary>
        /// Disconnect from the host
        /// </summary>
        void Disconnect();
    }
}
