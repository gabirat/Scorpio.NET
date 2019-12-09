using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Scorpio.Messaging.Abstractions;
using System;
using System.Threading.Tasks;

namespace Scorpio.Api.Hubs
{
    public class MainHub : Hub
    {
        private readonly IEventBus _eventBus;
        private readonly ILogger<MainHub> _logger;

        #region Constructor
        public MainHub(IEventBus eventBus, ILogger<MainHub> logger)
        {
            _eventBus = eventBus;
            _logger = logger;
        }
        #endregion

        #region Basic configuration
        public override Task OnConnectedAsync()
        {
            _logger.LogInformation($"New SignalR connection: {Context?.ConnectionId}");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation($"SignalR user disconnected: {exception?.ToString()}");
            return base.OnDisconnectedAsync(exception);
        }
        #endregion

        #region Following methods are callable from the UI via SignalR
        public void Data(object data)
        {
            Console.WriteLine($"Received SignalR data: {JsonConvert.SerializeObject(data)}");
        }
        #endregion
    }
}
