using System.Threading.Tasks;

namespace Newbe.Claptrap
{
    public abstract class NormalEventHandler<TStateData, TEventData> : IEventHandler
        where TStateData : class, IStateData
        where TEventData : class, IEventData

    {
        public virtual ValueTask DisposeAsync()
        {
            Dispose();
            return ValueTask.CompletedTask;
        }

        public async Task<IState> HandleEvent(IEventContext eventContext)
        {
            var stateData = (TStateData) eventContext.State.Data;
            var eventData = (TEventData) eventContext.Event.Data;
            await HandleEvent(stateData, eventData, eventContext);
            return eventContext.State;
        }

        // ReSharper disable once MemberCanBeProtected.Global
        public abstract ValueTask HandleEvent(TStateData stateData, TEventData eventData, IEventContext eventContext);

        public void Dispose()
        {
        }
    }
}