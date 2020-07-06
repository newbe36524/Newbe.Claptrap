using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.Core.Impl
{
    public abstract class EventHandlerFLowBase : IEventHandlerFLow
    {
        private readonly IStateAccessor _stateAccessor;
        private readonly IStateHolder _stateHolder;
        private readonly IEventHandlerFactory _eventHandlerFactory;
        private readonly IStateRestorer _stateRestorer;
        private readonly IStateSavingFlow _stateSavingFlow;
        private readonly StateRecoveryOptions _stateRecoveryOptions;
        private readonly ILogger _logger;

        private IDisposable _eventHandleFlow = null!;
        private readonly Subject<EventItem> _incomingEventsSeq;

        protected IState State
        {
            get => _stateAccessor.State;
            private set => _stateAccessor.State = value;
        }

        protected EventHandlerFLowBase(
            IStateAccessor stateAccessor,
            IStateHolder stateHolder,
            IEventHandlerFactory eventHandlerFactory,
            IStateRestorer stateRestorer,
            IStateSavingFlow stateSavingFlow,
            StateRecoveryOptions stateRecoveryOptions,
            ILogger logger)
        {
            _stateAccessor = stateAccessor;
            _stateHolder = stateHolder;
            _eventHandlerFactory = eventHandlerFactory;
            _stateRestorer = stateRestorer;
            _stateSavingFlow = stateSavingFlow;
            _stateRecoveryOptions = stateRecoveryOptions;
            _logger = logger;
            _incomingEventsSeq = new Subject<EventItem>();
        }

        public void Activate()
        {
            _eventHandleFlow = _incomingEventsSeq
                .Select(item => Observable.FromAsync(async () =>
                {
                    try
                    {
                        await HandleCoreAsync(item.Event);
                        item.TaskCompletionSource.SetResult(0);
                    }
                    catch (Exception e)
                    {
                        item.TaskCompletionSource.SetException(e);
                    }
                }))
                .Concat()
                .Subscribe(_ => { },
                    ex => { _logger.LogError(ex, "thrown a exception while handling event"); });
        }

        public virtual void Deactivate()
        {
            _eventHandleFlow?.Dispose();
            _incomingEventsSeq?.Dispose();
        }

        /// <summary>
        /// Handle incoming event
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        protected abstract Task HandleCoreAsync(IEvent @event);

        /// <summary>
        /// On before event handling
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected abstract Task OnBeforeEventHandling(EventHandleFlowContext context);

        /// <summary>
        /// On after event handled
        /// </summary>
        /// <param name="context"></param>
        /// <param name="newState"></param>
        /// <returns></returns>
        protected abstract Task OnAfterEventHandled(EventHandleFlowContext context, IState newState);

        /// <summary>
        /// handle event and update state.
        /// state will be restore if some exception thrown.
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        protected async Task HandleEventAndUpdateStateAsync(IEvent @event)
        {
            var context = CreateContext();
            try
            {
                await HandleEventCoreAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                await HandleException(e).ConfigureAwait(false);
                throw;
            }

            async Task HandleEventCoreAsync()
            {
                await OnBeforeEventHandling(context);
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
                await OnAfterEventHandled(context, nextState);
                _logger.LogDebug("state version updated : {version}", State.Version);
            }

            async Task HandleException(Exception e)
            {
                _logger.LogWarning(e,
                    "there is an exception when handle event : {@event} . start to recover state as strategy : {strategy}",
                    context.Event,
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
                    Event = @event,
                };
                re.EventContext = new EventContext(re.Event, re.NowState);
                re.EventHandler = CreateHandler(re.EventContext);
                return re;
            }
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

        private IEventHandler CreateHandler(IEventContext eventContext)
        {
            _logger.LogTrace("creating event handler");
            var handler = _eventHandlerFactory.Create(eventContext);
            _logger.LogTrace("created event handler : {handler}", handler);
            return handler;
        }

        protected struct EventHandleFlowContext
        {
            public IState NowState { get; set; }
            public IEvent Event { get; set; }
            public IEventContext EventContext { get; set; }
            public IEventHandler EventHandler { get; set; }
        }

        private struct EventItem
        {
            public IEvent Event { get; set; }
            public TaskCompletionSource<int> TaskCompletionSource { get; set; }
        }
    }
}