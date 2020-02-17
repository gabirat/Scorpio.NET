using System;

namespace Scorpio.Messaging.Sockets.Workers
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public Envelope Envelope { get; set; }

        public MessageReceivedEventArgs(Envelope envelope)
        {
            Envelope = envelope;
        }

        public MessageReceivedEventArgs()
        {
        }
    }
}
