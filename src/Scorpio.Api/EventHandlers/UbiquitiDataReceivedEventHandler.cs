using Microsoft.AspNetCore.SignalR;
using Scorpio.Api.Hubs;
using Scorpio.Messaging.Abstractions;
using Scorpio.Messaging.Messages;
using System.Threading.Tasks;

namespace Scorpio.Api.EventHandlers
{
    public class UbiquitiDataReceivedEventHandler : IIntegrationEventHandler<UbiquitiDataReceivedEvent>
    {
        private readonly IHubContext<MainHub> _hubContext;

        public UbiquitiDataReceivedEventHandler(IHubContext<MainHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(UbiquitiDataReceivedEvent @event)
        {
            // Notify web page via SignalR
            await _hubContext.Clients.All.SendAsync(Constants.Topics.Ubiquiti, @event.Data);
        }
    }
}
