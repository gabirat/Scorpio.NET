namespace Scorpio.Messaging.Sockets.Workers
{
    internal interface IWorker
    {
        void Start();
        void Stop();
        void Cancel();
        void Close();
    }
}
