using System;
using System.Net.Sockets;

namespace Scorpio.Messaging.Sockets
{
    public interface ISocketClient
    {
        event EventHandler<EventArgs> Connected;
        event EventHandler<EventArgs> Disconnected;

        bool IsConnected { get; }
        bool TryConnect();
        void Disconnect();
        NetworkStream Stream { get; }
    }
}
