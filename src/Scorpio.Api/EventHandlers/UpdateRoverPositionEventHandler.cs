using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Scorpio.Api.Events;
using Scorpio.Api.Hubs;
using Scorpio.Messaging.Abstractions;
using System;
using System.Threading.Tasks;

namespace Scorpio.Api.EventHandlers
{
    public class UpdateRoverPositionEventHandler : IIntegrationEventHandler<UpdateRoverPositionEvent>
    {
        readonly IHubContext<MainHub> _hubContext;

        public UpdateRoverPositionEventHandler(IHubContext<MainHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(UpdateRoverPositionEvent @event)
        {
            Console.WriteLine("Received event");
            Console.WriteLine(JsonConvert.SerializeObject(@event));

            await _hubContext.Clients.All.SendAsync("data", @event);
        }
    }
}
