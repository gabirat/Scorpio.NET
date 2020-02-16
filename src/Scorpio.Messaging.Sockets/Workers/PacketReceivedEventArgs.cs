using System;

namespace Scorpio.Messaging.Sockets.Workers
{
    internal class PacketReceivedEventArgs : EventArgs
    {
        public byte[] Packet { get; set; }
        public int Length { get; set; }

        public PacketReceivedEventArgs()
        {
        }

        public PacketReceivedEventArgs(byte[] packet, int length)
        {
            Packet = packet;
            Length = Length;
        }
    }
}
