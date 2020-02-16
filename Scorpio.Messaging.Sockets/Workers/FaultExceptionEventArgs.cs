using System;
using System.IO;

namespace Scorpio.Messaging.Sockets.Workers
{
    public class FaultExceptionEventArgs : ErrorEventArgs
    {
        public FaultExceptionEventArgs(Exception exception) : base(exception)
        {
        }
    }
}
