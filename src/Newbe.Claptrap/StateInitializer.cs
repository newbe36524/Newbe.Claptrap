using System.Threading.Tasks;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.EventHandler;
using Newbe.Claptrap.EventStore;
using Newbe.Claptrap.StateInitializer;
using Newbe.Claptrap.StateStore;

namespace Newbe.Claptrap
{
    public class EventSourcingStateInitializer : IStateInitializer
    {
        private readonly EventSourcingStateBuilderOptions _options;
        private readonly IEventStore _eventStore;
        private readonly IStateStore _stateStore;
        private readonly IStateDataFactory _stateDataFactory;
        private readonly IStateDataUpdaterFactory _stateDataUpdaterFactory;


        public EventSourcingStateInitializer(
            EventSourcingStateBuilderOptions options,
            IActorIdentity actorIdentity,
            IEventStore eventStore,
            IStateStore stateStore,
            IStateDataFactory stateDataFactory,
            IStateDataUpdaterFactory stateDataUpdaterFactory)
        {
            _options = options;
            ActorIdentity = actorIdentity;
            _eventStore = eventStore;
            _stateStore = stateStore;
            _stateDataFactory = stateDataFactory;
            _stateDataUpdaterFactory = stateDataUpdaterFactory;
        }

        public IActorIdentity ActorIdentity { get; }

        public Task<IState> InitializeAsync()
        {
            return BuildState(ActorIdentity);
        }

        public async Task<IState> BuildState(IActorIdentity identity)
        {
            var state = await _stateStore.GetStateSnapshot();

            // there is no state from state store, just create a default state from factory 
            if (state == null)
            {
                var stateData = await _stateDataFactory.CreateInitialState();
                state = new DataState(identity, (IStateData) stateData, 1);
            }

            ulong count;
            do
            {
                count = 0;
                var endVersion = state.Version + _options.RestoreEventVersionCountPerTime;
                var events = await _eventStore.GetEvents(state.Version, endVersion);
                foreach (var @event in events)
                {
                    var handler = _stateDataUpdaterFactory.Create(state, @event);
                    handler.Update(state.Data, @event.Data);
                    count++;
                }
            } while (count >= _options.RestoreEventVersionCountPerTime);

            return state;
        }
    }
}