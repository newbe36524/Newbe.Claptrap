using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.EventHandler
{
    public interface IStateDataUpdater
    {
        void Update(IStateData state, IEventData @event);
    }
}