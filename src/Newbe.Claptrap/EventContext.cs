using Newbe.Claptrap.Context;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap
{
    public class EventContext : IEventContext
    {
        public EventContext(IEvent @event, IActorContext actorContext)
        {
            Event = @event;
            ActorContext = actorContext;
        }

        public IEvent Event { get; }
        public IActorContext ActorContext { get; }
    }
}