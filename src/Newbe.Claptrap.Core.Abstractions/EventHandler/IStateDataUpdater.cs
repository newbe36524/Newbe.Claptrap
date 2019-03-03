using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.EventHandler
{
    public interface IStateDataUpdater
    {
        void UpdateStateData(IStateData state, IEventData @event);
    }
}