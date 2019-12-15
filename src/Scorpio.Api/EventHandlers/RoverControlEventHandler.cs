using Scorpio.Messaging.Abstractions;
using Scorpio.Messaging.Messages;
using System;
using System.Threading.Tasks;

namespace Scorpio.Api.EventHandlers
{
    public class RoverControlEventHandler : IIntegrationEventHandler<RoverControlCommand>
    {
        public Task Handle(RoverControlCommand @event)
        {
            Console.WriteLine($"ACC {@event.Acc}  DIR {@event.Dir}");
            return Task.CompletedTask;
        }
    }
}
