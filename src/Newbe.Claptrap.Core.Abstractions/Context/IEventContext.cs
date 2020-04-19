using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.Context
{
    public interface IEventContext
    {
        IEvent Event { get; }
        IState State { get; }
    }
}