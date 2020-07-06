using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.Core.Impl
{
    public class MinionEventHandlerFLow : IEventHandlerFLow
    {
        private readonly IStateAccessor _stateAccessor;
        private readonly IStateHolder _stateHolder;
        private readonly IEventLoader _eventLoader;
        private readonly IEventHandlerFactory _eventHandlerFactory;
        private readonly IStateRestorer _stateRestorer;
        private readonly IStateSavingFlow _stateSavingFlow;
        private readonly StateRecoveryOptions _stateRecoveryOptions;
        private readonly EventLoadingOptions _eventLoadingOptions;
        private readonly ILogger<MasterEventHandlerFLow> _logger;

        private IDisposable _eventHandleFlow = null!;
        private readonly Subject<EventItem> _incomingEventsSeq;

        private IState State
        {
            get => _stateAccessor.State;
            set => _stateAccessor.State = value;
        }

        public MinionEventHandlerFLow(
            IStateAccessor stateAccessor,
            IStateHolder stateHolder,
            IEventLoader eventLoader,
            IEventHandlerFactory eventHandlerFactory,
            IStateRestorer stateRestorer,
            IStateSavingFlow stateSavingFlow,
            StateRecoveryOptions stateRecoveryOptions,
            EventLoadingOptions eventLoadingOptions,
            ILogger<MasterEventHandlerFLow> logger)
        {
            _stateAccessor = stateAccessor;
            _stateHolder = stateHolder;
            _eventLoader = eventLoader;
            _eventHandlerFactory = eventHandlerFactory;
            _stateRestorer = stateRestorer;
            _stateSavingFlow = stateSavingFlow;
            _stateRecoveryOptions = stateRecoveryOptions;
            _eventLoadingOptions = eventLoadingOptions;
            _logger = logger;
            _incomingEventsSeq = new Subject<EventItem>();
        }

        public void Activate()
        {
            _eventHandleFlow = _incomingEventsSeq
                .Select(item => Observable.FromAsync(() => HandleCoreAsync(item)))
                .Concat()
                .Subscribe(_ => { },
                    ex => { _logger.LogError(ex, "thrown a exception while handling event"); });
        }

        private async Task HandleCoreAsync(EventItem sourceItem)
        {
            try
            {
                await foreach (var one in AllEvents(sourceItem.Event))
                {
                    await HandleOne(one);
                }

                sourceItem.TaskCompletionSource.SetResult(0);
            }
            catch (Exception e)
            {
                sourceItem.TaskCompletionSource.SetException(e);
                throw;
            }

            async IAsyncEnumerable<IEvent> AllEvents(IEvent evt)
            {
                if (State.NextVersion == evt.Version)
                {
                    yield return evt;
                    yield break;
                }

                if (evt.Version < State.NextVersion)
                {
                    yield break;
                }

                var step = _eventLoadingOptions.LoadingCountInOneBatch;
                var versionCount = evt.Version - State.NextVersion;
                var pageCount = (int) Math.Ceiling(versionCount * 1.0 / step);
                for (var i = 0; i < pageCount; i++)
                {
                    // State.NextVersion will change after move to next page, so i is not used in this loop
                    var left = State.NextVersion;
                    var right = Math.Min(State.NextVersion + step, evt.Version);
                    var events = await _eventLoader.GetEventsAsync(left, right);
                    foreach (var eventItem in events)
                    {
                        yield return eventItem;
                    }
                }

                yield return evt;
            }
        }

        private async Task HandleOne(IEvent evt)
        {
            var context = CreateContext();
            try
            {
                await HandleEventCoreAsync(context).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                await HandleException(e).ConfigureAwait(false);
                throw;
            }

            async Task HandleEventCoreAsync(EventHandleFlowContext context)
            {
                IState nextState;
                await using (var handler = context.EventHandler)
                {
                    nextState = await handler.HandleEvent(context.EventContext)
                        .ConfigureAwait(false);
                }

                _logger.LogDebug("event handled and updating state");
                _logger.LogDebug("start update to {@state}", nextState);
                State = nextState;
                State.IncreaseVersion();
                _stateSavingFlow.OnNewStateCreated(State);
                _logger.LogDebug("state version updated : {version}", State.Version);
            }

            async Task HandleException(Exception e)
            {
                _logger.LogWarning(e,
                    "there is an exception when handle event : {@event} . start to recover state as strategy : {strategy}",
                    evt,
                    _stateRecoveryOptions.StateRecoveryStrategy);
                switch (_stateRecoveryOptions.StateRecoveryStrategy)
                {
                    case StateRecoveryStrategy.FromStateHolder:
                        State = context.NowState;
                        break;
                    case StateRecoveryStrategy.FromStore:
                        await _stateRestorer.RestoreAsync();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            EventHandleFlowContext CreateContext()
            {
                var re = new EventHandleFlowContext
                {
                    NowState = _stateHolder.DeepCopy(State),
                    Event = evt,
                };
                if (re.Event.Version != re.NowState.NextVersion)
                {
                    throw new VersionErrorException(re.NowState.Version, re.Event.Version);
                }

                re.EventContext = new EventContext(re.Event, re.NowState);
                re.EventHandler = CreateHandler(re.EventContext);
                return re;
            }
        }

        public void Deactivate()
        {
            _eventHandleFlow?.Dispose();
            _incomingEventsSeq?.Dispose();
        }

        public Task OnNewEventReceived(IEvent @event)
        {
            var eventItem = new EventItem
            {
                Event = @event,
                TaskCompletionSource = new TaskCompletionSource<int>()
            };

            _incomingEventsSeq.OnNext(eventItem);

            var task = eventItem.TaskCompletionSource.Task;
            return task;
        }

        private class EventHandleFlowContext
        {
            public IState NowState { get; set; } = null!;
            public IEvent Event { get; set; } = null!;
            public IEventContext EventContext { get; set; } = null!;
            public IEventHandler EventHandler { get; set; } = null!;
        }

        private IEventHandler CreateHandler(IEventContext eventContext)
        {
            _logger.LogTrace("creating event handler");
            var handler = _eventHandlerFactory.Create(eventContext);
            _logger.LogTrace("created event handler : {handler}", handler);
            return handler;
        }

        private struct EventItem
        {
            public IEvent Event { get; set; }
            public TaskCompletionSource<int> TaskCompletionSource { get; set; }
        }
    }
}