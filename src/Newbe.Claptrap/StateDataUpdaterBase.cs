using Newbe.Claptrap.Core;
using Newbe.Claptrap.EventHandler;

namespace Newbe.Claptrap
{
    public abstract class StateDataUpdaterBase<TStateData, TEventData>
        : IStateDataUpdater
        where TStateData : class, IStateData
        where TEventData : class, IEventData
    {
        void IStateDataUpdater.Update(IStateData stateData, IEventData eventData)
        {
            UpdateState((TStateData) stateData, (TEventData) eventData);
        }

        public abstract void UpdateState(TStateData stateData, TEventData eventData);
    }
}