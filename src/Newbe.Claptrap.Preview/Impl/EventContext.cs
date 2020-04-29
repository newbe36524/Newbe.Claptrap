using Newbe.Claptrap.Preview.Abstractions.Components;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Impl
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