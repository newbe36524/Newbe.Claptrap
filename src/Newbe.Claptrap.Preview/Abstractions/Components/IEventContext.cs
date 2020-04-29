using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Abstractions.Components
{
    public interface IEventContext
    {
        IEvent Event { get; }
        IState State { get; }
    }
}