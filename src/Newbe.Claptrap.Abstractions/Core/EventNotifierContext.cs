namespace Newbe.Claptrap
{
    public record EventNotifierContext : IEventNotifierContext
    {
        public IEvent Event { get; init; }
        public IState CurrentState { get; init; }
        public IState OldState { get; init; }
    }
}