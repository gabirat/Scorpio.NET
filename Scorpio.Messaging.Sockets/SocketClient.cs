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
        public event EventHandler<EventArgs> Connected;
        public event EventHandler<EventArgs> Disconnected;
        public bool IsConnected => _client != null && _client.Connected;
        private TcpClient _client;

        // TODO lock
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
            private set => _stream = value;
        }

        private readonly ILogger<SocketClient> _logger;
        private readonly SocketConfiguration _options;
        private readonly object _syncLock = new object();

        public SocketClient(ILogger<SocketClient> logger, SocketConfiguration options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public SocketClient(ILogger<SocketClient> logger, IOptions<SocketConfiguration> options)
            : this(logger, options.Value)
        {
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
                    Connected?.Invoke(this, EventArgs.Empty);

                    var msg = $"Socket connection acquired a connection {_options.Host}:{_options.Port};";
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
                Connected?.Invoke(this, EventArgs.Empty);
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
            Disconnected?.Invoke(this, EventArgs.Empty);
            _client?.Close();
            _client = null;
            _stream = null;
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
