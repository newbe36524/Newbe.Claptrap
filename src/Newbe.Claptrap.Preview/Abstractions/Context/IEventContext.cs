using Newbe.Claptrap.Preview.Core;

namespace Newbe.Claptrap.Preview.Context
{
    public interface IEventContext
    {
        IEvent Event { get; }
        IState State { get; }
    }
}