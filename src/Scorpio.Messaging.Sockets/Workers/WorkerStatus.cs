namespace Scorpio.Messaging.Sockets.Workers
{
    internal enum WorkerStatus : byte
    {
        Running = 1,
        Stopped = 5,
        Faulted = 10
    }
}