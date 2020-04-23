using System.Threading.Tasks;
using Newbe.Claptrap.Context;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.EventHandler;

namespace Newbe.Claptrap.Demo
{
    public abstract class ClaptrapEventHandler<TStateData, TEventData> : IEventHandler
    {
        public virtual ValueTask DisposeAsync()
        {
            return new ValueTask();
        }

        public async Task<IState> HandleEvent(IEventContext eventContext)
        {
            await HandleEventCore(
                (TStateData) eventContext.State.Data,
                (TEventData) eventContext.Event.Data,
                eventContext);
            return eventContext.State;
        }

        public abstract ValueTask HandleEventCore(TStateData stateData, TEventData eventData, IEventContext context);
    }
}