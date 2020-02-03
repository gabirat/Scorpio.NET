using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Polly;

namespace Scorpio.Messaging.RabbitMQ
{
    public class RabbitMqConnection : IRabbitMqConnection
    {
        public event EventHandler<EventArgs> OnConnected;
        public event EventHandler<EventArgs> OnDisconnected;

        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<RabbitMqConnection> _logger;
        private IConnection _connection;
        private bool _disposed;
        private readonly object _syncLock = new object();

        public RabbitMqConnection(IConnectionFactory connectionFactory, ILogger<RabbitMqConnection> logger)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool IsConnected => _connection != null && _connection.IsOpen;

        public IModel CreateModel()
        {
            if (!IsConnected)
            {
               throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }

            return _connection.CreateModel();
        }

        public bool TryConnect() => TryConnect(CancellationToken.None);

        public bool TryConnect(CancellationToken cancellationToken)
        {
            lock (_syncLock)
            {
                if (IsConnected)
                {
                    _logger.LogWarning("Already connected!");
                    return true;
                }

                if (_connectionFactory is ConnectionFactory cf)
                {
                    _logger.LogInformation($"RabbitMQ connecting to: {cf.Endpoint}");
                }

                Policy
                    .Handle<BrokerUnreachableException>()
                    .Or<SocketException>()
                    .WaitAndRetryForever(retryNumber =>
                    {
                        _logger.LogCritical($"Reconnecting: {retryNumber}");
                        return TimeSpan.FromSeconds(3);
                    })
                    .Execute(() =>
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            _logger.LogInformation("Connection stop request");
                            return;
                        }
                        _connection = _connectionFactory.CreateConnection();
                    });

                if (IsConnected)
                {
                    _connection.ConnectionShutdown += OnConnectionShutdown;
                    _connection.CallbackException += OnCallbackException;
                    _connection.ConnectionBlocked += OnConnectionBlocked;
                    OnConnected?.Invoke(this, EventArgs.Empty);

                    _logger.LogInformation($"RabbitMQ persistent connection acquired a connection {_connection.Endpoint.HostName} and is subscribed to failure events");

                    return true;
                }

                _logger.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened");

                return false;
            }
        }
        
        protected void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");

            TryConnect();
        }

        protected void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");

            TryConnect();
        }

        protected void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if (_disposed) return;

            OnDisconnected?.Invoke(this, EventArgs.Empty);

            _logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");

            TryConnect();
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            try
            {
                _connection?.Close();
                _connection?.Dispose();
                _logger.LogInformation("Disposing RabbitMQ connection...");
            }
            catch (IOException ex)
            {
                _logger.LogCritical(ex.ToString());
            }
        }
    }
}
