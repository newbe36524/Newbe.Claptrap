namespace Newbe.Claptrap
{
    public interface IEventContext
    {
        IEvent Event { get; }
        IState State { get; }
    }
}