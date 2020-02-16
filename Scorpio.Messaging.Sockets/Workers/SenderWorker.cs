using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace Scorpio.Messaging.Sockets.Workers
{
    internal class SenderWorker : WorkerBase
    {
        internal event EventHandler<FaultExceptionEventArgs> SenderNetworkFault;
        private readonly ConcurrentQueue<byte[]> _queue;
        
        public SenderWorker(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _queue = new ConcurrentQueue<byte[]>();
        }

        public void Enqueue(byte[] data)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            _queue.Enqueue(data);
        }

        protected override void DoWork()
        {
            while (NetworkStream != null && NetworkStream.CanWrite && _queue.TryDequeue(out byte[] data))
            {
                try
                {
                    NetworkStream.Write(data, 0, data.Length);
                }
                catch (IOException ex)
                {
                    var eventArgs = new FaultExceptionEventArgs(ex);
                    SenderNetworkFault?.Invoke(this, eventArgs);
                }
            }
        }
    }
}
