using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Scorpio.Api.Events;
using Scorpio.Messaging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scorpio.Api.Hubs
{
    public class MainHub : Hub
    {
        private readonly IEventBus _eventBus;
        public MainHub(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine("OnConnectedAsync" + Context.ConnectionId);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        public void Data(IList<double> data)
        {
            _eventBus.Publish(new UpdateRoverPositionEvent(data.FirstOrDefault().ToString(), data.FirstOrDefault().ToString()));
            Console.WriteLine($"Received SignalR data: {JsonConvert.SerializeObject(data)}");
            //await Clients.All.SendAsync("data", "pong");
            //await Clients.All.SendAsync("data",  data);
        }
    }
}
