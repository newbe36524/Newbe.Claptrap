using System.Threading.Tasks;
using Newbe.Claptrap.Context;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.EventHandler;

namespace Newbe.Claptrap
{
    public abstract class MinionEventHandlerBase<TStateData, TEventData> : IEventHandler
        where TStateData : class, IStateData
        where TEventData : class, IEventData
    {
        public virtual ValueTask DisposeAsync()
        {
            return new ValueTask();
        }

        public Task HandleEvent(IEventContext eventContext)
        {
            return HandleEventCore((TStateData) eventContext.ActorContext.State.Data,
                (TEventData) eventContext.Event.Data);
        }

        public abstract Task HandleEventCore(TStateData state, TEventData @event);
    }
}