using System.Net.Sockets;

namespace Scorpio.Messaging.Sockets
{
    public interface ISocketClient
    {
        bool IsConnected { get; }
        bool TryConnect();
        void Disconnect();
        NetworkStream Stream { get; }
    }
}
