using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Scorpio.Api.Events;
using Scorpio.Api.Hubs;
using Scorpio.Messaging.Abstractions;
using System;
using System.Threading.Tasks;

namespace Scorpio.Api.EventHandlers
{
    public class TestEventHandler : IIntegrationEventHandler<Test>
    {
        readonly IHubContext<MainHub> _hubContext;

        public TestEventHandler(IHubContext<MainHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(Test @event)
        {
            Console.WriteLine(JsonConvert.SerializeObject(@event));
        }
    }
}
