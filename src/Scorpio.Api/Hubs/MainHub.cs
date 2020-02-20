using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Scorpio.Messaging.Abstractions;
using Scorpio.Messaging.Messages;
using System;
using System.Collections.Generic;
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

        [HubMethodName("RoverControlCommand")]
        public void RoverControlCommand(Dictionary<string, object> data)
        {
            if (!data.ContainsKey("acc") || !data.ContainsKey("dir")) return;

            if (float.TryParse(data["acc"].ToString(), out var acc) &&
                float.TryParse(data["dir"].ToString(), out var dir))
            {
                var command = new RoverControlCommand(dir, acc);
                _logger.LogInformation($"Received SignalR data: {JsonConvert.SerializeObject(command)}");
                _eventBus.Publish(command);
            }

        }
        #endregion
    }
}
