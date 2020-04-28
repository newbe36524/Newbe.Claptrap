using Newbe.Claptrap.Preview.Context;
using Newbe.Claptrap.Preview.Core;

namespace Newbe.Claptrap.Preview
{
    public class EventContext : IEventContext
    {
        public EventContext(IEvent @event, IState actorContext)
        {
            Event = @event;
            State = actorContext;
        }

        public IEvent Event { get; }
        public IState State { get; }
    }
}