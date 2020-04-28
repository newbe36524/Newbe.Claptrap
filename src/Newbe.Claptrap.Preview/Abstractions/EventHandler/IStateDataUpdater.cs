using Newbe.Claptrap.Preview.Core;

namespace Newbe.Claptrap.Preview.EventHandler
{
    public interface IStateDataUpdater
    {
        void Update(IStateData state, IEventData @event);
    }
}