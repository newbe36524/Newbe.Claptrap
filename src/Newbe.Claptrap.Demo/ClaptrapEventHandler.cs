using System.Threading.Tasks;
using Newbe.Claptrap.Preview.Context;
using Newbe.Claptrap.Preview.Core;
using Newbe.Claptrap.Preview.EventHandler;

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