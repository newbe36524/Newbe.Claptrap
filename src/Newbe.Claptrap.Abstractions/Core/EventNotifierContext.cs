namespace Newbe.Claptrap
{
    public class EventNotifierContext : IEventNotifierContext
    {
        public IEvent Event { get; set; } = null!;
        public IState CurrentState { get; set; } = null!;
        public IState OldState { get; set; } = null!;
    }
}