using System.Threading.Tasks;

namespace Newbe.Claptrap
{
    public abstract class NormalEventHandler<TStateData, TEventData> : IEventHandler
        where TStateData : class, IStateData
        where TEventData : class, IEventData

    {
        public virtual ValueTask DisposeAsync()
        {
            return new ValueTask();
        }

        public async Task<IState> HandleEvent(IEventContext eventContext)
        {
            var stateData = (TStateData) eventContext.State.Data;
            var eventData = (TEventData) eventContext.Event.Data;
            await HandleEvent(stateData, eventData, eventContext);
            return eventContext.State;
        }

        public abstract ValueTask HandleEvent(TStateData stateData, TEventData eventData, IEventContext eventContext);
    }
}