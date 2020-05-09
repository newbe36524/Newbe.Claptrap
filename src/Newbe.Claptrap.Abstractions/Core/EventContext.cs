namespace Newbe.Claptrap
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