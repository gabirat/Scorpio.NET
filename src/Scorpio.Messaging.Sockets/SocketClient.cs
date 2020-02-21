﻿using Autofac;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Scorpio.Messaging.Abstractions;
using Scorpio.Messaging.Sockets.Workers;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Scorpio.Messaging.Sockets
{
    public class SocketClient : ISocketClient
    {
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<EventArgs> Connected;
        public event EventHandler<EventArgs> Disconnected;
        public bool IsConnected => _client != null && _client.Connected;
        private readonly object _sendSyncLock;
        private TcpClient _client;
        private NetworkWorkersFacade _workersFacade;
        private readonly ILifetimeScope _autofac;

        private NetworkStream _stream;
        public NetworkStream Stream
        {
            get
            {
                if (_client != null && _client.Connected)
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

        public SocketClient(ILifetimeScope autofac, ILogger<SocketClient> logger, IOptions<SocketConfiguration> options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _autofac = autofac ?? throw new ArgumentNullException(nameof(autofac));
            _sendSyncLock = new object();
        }


        public bool TryConnect(CancellationToken ct)
        {
            lock (_syncLock)
            {
                try
                {
                    Policy
                        .Handle<SocketException>()
                        .WaitAndRetryForever(retryNumber =>
                        {
                            _logger.LogCritical($"Reconnecting: {retryNumber}");
                            return TimeSpan.FromSeconds(2);
                        })
                        .Execute(token =>
                        {
                            Connect();
                            token.ThrowIfCancellationRequested();
                        }, ct);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning($"Operation {nameof(TryConnect)} was cancelled...");
                    throw;
                }

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

        public void TryConnect() => TryConnect(CancellationToken.None);

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

        public void Enqueue(IntegrationEvent @event)
        {
            lock (_sendSyncLock)
            {
                if (!IsConnected)
                    TryConnect();

                if (Stream != null
                    && Stream.CanWrite
                    && _workersFacade.SenderStatus == WorkerStatus.Running)
                {
                    byte[] message = Envelope.Build(@event).Serialize();
                    _workersFacade.Enqueue(message);
                }
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
            CreateWorkersFacade();
            Connected?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnDisconnected()
        {
            DestroyWorkerFacade();
            Disconnected?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void CreateWorkersFacade()
        {
            _logger.LogInformation("Creating workers facade...");
            _workersFacade = new NetworkWorkersFacade(_autofac);
            _workersFacade.NetworkWorkerFaulted += WorkersFacade_NetworkWorkerFaulted;
            _workersFacade.PacketReceived += WorkersFacade_PacketReceived;
            _workersFacade.NetworkStream = Stream;
            _workersFacade.Start();
        }

        protected virtual void DestroyWorkerFacade()
        {
            if (_workersFacade is null) return;

            _logger.LogInformation("Destroying workers facade...");

            _workersFacade.NetworkWorkerFaulted -= WorkersFacade_NetworkWorkerFaulted;
            _workersFacade.PacketReceived -= WorkersFacade_PacketReceived;
            _workersFacade?.Stop();
            _workersFacade = null;
        }
        private void WorkersFacade_NetworkWorkerFaulted(object sender, FaultExceptionEventArgs e)
        {
            var ex = e?.GetException();
            _logger.LogError($"{sender.GetType().FullName} faulted: " + ex?.Message, ex);

            lock (_sendSyncLock)
            {
                Disconnect();
                TryConnect();
            }
        }

        private void WorkersFacade_PacketReceived(object sender, PacketReceivedEventArgs e)
        {
            try
            {
                var envelope = Envelope.Deserialize(e.Packet);
                MessageReceived?.Invoke(this, new MessageReceivedEventArgs(envelope));
            }
            catch (JsonSerializationException)
            {
                _logger.LogWarning("Received message, but cannot deserialize (invalid protocol)");
            }
            catch (JsonReaderException)
            {
                _logger.LogWarning("Received message, but cannot deserialize (invalid protocol)");
            }
        }

        public void Dispose()
        {
            try
            {
                DestroyWorkerFacade();
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
