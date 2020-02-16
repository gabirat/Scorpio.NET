using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using System;
using System.Net;
using System.Net.Sockets;

namespace Scorpio.Messaging.Sockets
{
    public class SocketClient : ISocketClient
    {
        public event EventHandler<EventArgs> Connected;
        public event EventHandler<EventArgs> Disconnected;
        public bool IsConnected => _client != null && _client.Connected;

        private TcpClient _client;

        private NetworkStream _stream;
        public NetworkStream Stream
        {
            get
            {
                if (_client.Connected)
                {
                    _stream = _client?.GetStream();
                    return _stream;
                }
                return null;
            }
        }

        private readonly ILogger<SocketClient> _logger;
        private readonly SocketConfiguration _options;
        private readonly object _syncLock = new object();

        public SocketClient(ILogger<SocketClient> logger, IOptions<SocketConfiguration> options)
            : this(logger, options.Value)
        {
        }

        public SocketClient(ILogger<SocketClient> logger, SocketConfiguration options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? throw new ArgumentNullException(nameof(options));
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
                        return TimeSpan.FromSeconds(2);
                    })
                    .Execute(Connect);

                if (IsConnected)
                {
                    OnConnected();

                    var msg = $"Socket connection acquired a connection {_options.Host}:{_options.Port};";
                    _logger.LogInformation(msg);
                    return true;
                }

                _logger.LogCritical("FATAL: Socket connections could not connect");
                return false;
            }
        }

        private void Connect()
        {
            if (string.IsNullOrEmpty(_options.Host))
                throw new ArgumentNullException(nameof(_options.Host));

            if (_options.Port == 0)
                throw new ArgumentNullException(nameof(_options.Port));

            try
            {
                var hostIp = IPAddress.Parse(_options.Host);
                var endpoint = new IPEndPoint(hostIp, _options.Port);

                _client = new TcpClient();
                _client.Connect(endpoint);
                _client.ReceiveTimeout = 5000;
            }
            catch (SocketException ex)
            {
                _logger.LogError("Error while connecting to TCP server: " + ex.Message, ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while connecting to TCP server: " + ex.Message, ex);
            }
        }
        
        public void Disconnect()
        {
            OnDisconnected();
            _client?.Close();
            _client = null;
            _stream = null;
        }

        protected virtual void OnConnected()
        {
            Connected?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnDisconnected()
        {
            Disconnected?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            try
            {
                _client?.Dispose();
                Stream?.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.ToString());
            }
        }
    }
}
