using Newbe.Claptrap.Preview.Core;

namespace Newbe.Claptrap.Preview.EventHandler
{
    public interface IStateDataUpdaterFactory
    {
        IStateDataUpdater Create(IState state, IEvent @event);
    }
}