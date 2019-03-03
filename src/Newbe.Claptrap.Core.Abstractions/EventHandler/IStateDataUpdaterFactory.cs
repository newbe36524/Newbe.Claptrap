using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.EventHandler
{
    public interface IStateDataUpdaterFactory
    {
        IStateDataUpdater Create(IState state, IEvent @event);
    }
}