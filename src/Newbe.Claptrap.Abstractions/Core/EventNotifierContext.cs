namespace Newbe.Claptrap
{
    public class EventNotifierContext : IEventNotifierContext
    {
        public IEvent Event { get; set; }
        public IState CurrentState { get; set; }
        public IState OldState { get; set; }
    }
}