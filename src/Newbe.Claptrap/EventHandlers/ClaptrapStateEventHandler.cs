using System;
using System.Threading.Tasks;
using Newbe.Claptrap.Context;
using Newbe.Claptrap.EventHandler;
using Newbe.Claptrap.EventStore;
using Newbe.Claptrap.StateStore;

namespace Newbe.Claptrap.EventHandlers
{
    public class ClaptrapStateEventHandler : IEventHandler
    {
        private readonly IEventStore _eventStore;
        private readonly IStateStore _stateStore;
        private readonly IStateDataUpdaterFactory _stateDataUpdaterFactory;

        public ClaptrapStateEventHandler(
            IEventStore eventStore,
            IStateStore stateStore,
            IStateDataUpdaterFactory stateDataUpdaterFactory)
        {
            _eventStore = eventStore;
            _stateStore = stateStore;
            _stateDataUpdaterFactory = stateDataUpdaterFactory;
        }

        public async Task HandleEvent(IEventContext eventContext)
        {
            var @event = eventContext.Event;
            var state = eventContext.ActorContext.State;
            @event.Version = state.Version;
            var eventSavingResult = await _eventStore.SaveEvent(@event);
            switch (eventSavingResult)
            {
                case EventSavingResult.Exception:
                    // TODO log
                    break;
                case EventSavingResult.AlreadyAdded:
                    // event is already added, do not need to process it
                    break;
                case EventSavingResult.Success:
                    var handler = _stateDataUpdaterFactory.Create(state, @event);
                    handler.Update(state.Data, @event.Data);
                    state.IncreaseVersion();
                    // save latest state if event handle success
                    await _stateStore.Save(eventContext.ActorContext.State);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public ValueTask DisposeAsync()
        {
            return new ValueTask();
        }
    }
}