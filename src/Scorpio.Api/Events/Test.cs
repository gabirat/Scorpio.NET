using Scorpio.Messaging.Abstractions;

namespace Scorpio.Api.Events
{
    public class Test : IntegrationEvent
    {
        public object Dupa { get; set; }

        public Test(object dupa)
        {
            Dupa = dupa;
        }
    }
}
