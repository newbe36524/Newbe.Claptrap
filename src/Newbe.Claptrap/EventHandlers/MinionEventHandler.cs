using System.Threading.Tasks;
using Newbe.Claptrap.Context;
using Newbe.Claptrap.EventHandler;
using Newbe.Claptrap.StateStore;

namespace Newbe.Claptrap.EventHandlers
{
    public class MinionEventHandler : IEventHandler
    {
        private readonly IStateDataUpdaterFactory _stateDataUpdaterFactory;
        private readonly IStateStore _stateStore;

        public MinionEventHandler(
            IStateDataUpdaterFactory stateDataUpdaterFactory,
            IStateStore stateStore)
        {
            _stateDataUpdaterFactory = stateDataUpdaterFactory;
            _stateStore = stateStore;
        }

        public ValueTask DisposeAsync()
        {
            return new ValueTask();
        }

        public async Task HandleEvent(IEventContext eventContext)
        {
            var state = eventContext.ActorContext.State;
            var @event = eventContext.Event;
            if (@event.Version > state.Version)
            {
                var updater = _stateDataUpdaterFactory.Create(state, eventContext.Event);
                updater.UpdateStateData(state.Data, eventContext.Event.Data);
                state.IncreaseVersion();
                await _stateStore.Save(eventContext.ActorContext.State);
            }
        }
    }
}