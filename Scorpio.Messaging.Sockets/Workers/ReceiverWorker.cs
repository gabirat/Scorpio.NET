using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;

namespace Scorpio.Messaging.Sockets.Workers
{
    internal sealed class ReceiverWorker : WorkerBase
    {
        internal event EventHandler<PacketReceivedEventArgs> PacketReceived;
        internal event EventHandler<FaultExceptionEventArgs> ReceiverNetworkFault;

        protected override int WorkerSleepTime => 5;
        internal int MaxPacketLength { get; set; } = 1000;
        
        private readonly byte[] _data;

        public ReceiverWorker(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _data = new byte[MaxPacketLength];
        }

        protected override void DoWork()
        {
            if (!NetworkStream.CanRead || !NetworkStream.DataAvailable)
                return;

            try
            {
                // Receive header first - 4 bytes indicating total packet length
                var header = new byte[4];

                // Convert to integer with correct endianness
                NetworkStream.Read(header, 0, 4);

                int headerLengthInt = BitConverter.ToInt32(header, 0);
                int length = IPAddress.NetworkToHostOrder(headerLengthInt);

                // Receive actual packet
                NetworkStream.Read(_data, 0, length);
 
                // Invoke event
                var eventArgs = new PacketReceivedEventArgs(_data, length);
                PacketReceived?.Invoke(this, eventArgs);
            }
            catch (ArgumentOutOfRangeException)
            {
                Logger.LogWarning("Received message, length bytes error (invalid protocol)");
            }
            catch (IOException ex) 
            {
                // Critical fault - probably need to restart connection
                var eventArgs = new FaultExceptionEventArgs(ex);
                ReceiverNetworkFault?.Invoke(this, eventArgs);
            }
        }
    }
}
