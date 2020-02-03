using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;

namespace Scorpio.Messaging.Sockets
{
    public class SocketClient : ISocketClient
    {
        public event EventHandler<EventArgs> OnConnected;
        public event EventHandler<EventArgs> OnDisconnected;
        public bool IsConnected { get; private set; }
        private TcpClient _client;


        // TODO lock
        private NetworkStream _stream;
        public NetworkStream Stream
        {
            get
            {
                _stream = _client?.GetStream();
                return _stream;
            }
            private set => _stream = value;
        }

        private readonly ILogger<SocketClient> _logger;
        private readonly SocketConfiguration _options;
        private readonly object _syncLock = new object();
        private bool _disposed;

        public SocketClient(ILogger<SocketClient> logger, IOptions<SocketConfiguration> options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public bool TryConnect()
        {
            lock (_syncLock)
            {
                Policy
                    .Handle<SocketException>()
                    .WaitAndRetryForever(retryNumber =>
                    {
                        _logger.LogCritical($"Reconnecting: {retryNumber}");
                        return TimeSpan.FromSeconds(3);
                    })
                    .Execute(Connect);

                if (IsConnected)
                {
                    OnConnected?.Invoke(this, EventArgs.Empty);

                    var msg = $"Socket connection acquired a connection {_options.Host}:{_options.Host};";
                    _logger.LogInformation(msg);
                    return true;
                }

                _logger.LogCritical("FATAL ERROR: Socket connections could not connect");
                return false;
            }
        }

        private void Connect()
        {
            try
            {
                var hostIp = IPAddress.Parse(_options.Host);
                var endpoint = new IPEndPoint(hostIp, _options.Port);

                _client = new TcpClient();
                _client.Connect(endpoint);
                IsConnected = true;
            }
            catch (SocketException ex)
            {
                _logger.LogError("Error while connecting to TCP server", ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while connecting to TCP server", ex.Message, ex);
            }
        }

        public void Disconnect()
        {
            OnDisconnected?.Invoke(this, EventArgs.Empty);
            _client?.Close();
            _client = null;
            _stream= null;
            IsConnected = false;
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            try
            {
                _client?.Dispose();
                Stream?.Dispose();
            }
            catch (IOException ex)
            {
                _logger.LogCritical(ex.ToString());
            }
        }
    }
}
