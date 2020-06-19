using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.Core.Impl
{
    public class MasterEventHandlerFLow : IEventHandlerFLow
    {
        private readonly IStateAccessor _stateAccessor;
        private readonly IEventSaver _eventSaver;
        private readonly IStateHolder _stateHolder;
        private readonly IEventHandlerFactory _eventHandlerFactory;
        private readonly IStateRestorer _stateRestorer;
        private readonly IStateSavingFlow _stateSavingFlow;
        private readonly IEventHandledNotificationFlow _eventHandledNotificationFlow;
        private readonly StateRecoveryOptions _stateRecoveryOptions;
        private readonly ILogger<MasterEventHandlerFLow> _logger;

        private IDisposable _eventHandleFlow = null!;
        private readonly Subject<EventItem> _incomingEventsSeq;

        private IState State
        {
            get => _stateAccessor.State;
            set => _stateAccessor.State = value;
        }

        public MasterEventHandlerFLow(
            IStateAccessor stateAccessor,
            IEventSaver eventSaver,
            IStateHolder stateHolder,
            IEventHandlerFactory eventHandlerFactory,
            IStateRestorer stateRestorer,
            IStateSavingFlow stateSavingFlow,
            IEventHandledNotificationFlow eventHandledNotificationFlow,
            StateRecoveryOptions stateRecoveryOptions,
            ILogger<MasterEventHandlerFLow> logger)
        {
            _stateAccessor = stateAccessor;
            _eventSaver = eventSaver;
            _stateHolder = stateHolder;
            _eventHandlerFactory = eventHandlerFactory;
            _stateRestorer = stateRestorer;
            _stateSavingFlow = stateSavingFlow;
            _eventHandledNotificationFlow = eventHandledNotificationFlow;
            _stateRecoveryOptions = stateRecoveryOptions;
            _logger = logger;
            _incomingEventsSeq = new Subject<EventItem>();
        }

        public void Activate()
        {
            _eventHandleFlow = _incomingEventsSeq
                .Select(item => Observable.FromAsync(() => HandleCoreAsync(item)))
                .Concat()
                .Subscribe();
        }

        private async Task HandleCoreAsync(EventItem item)
        {
            var context = CreateContext();
            try
            {
                await HandleEventCoreAsync();
                context.TaskCompletionSource.SetResult(0);
            }
            catch (Exception e)
            {
                await HandleException(e);
                context.TaskCompletionSource.SetException(e);
            }

            async Task HandleEventCoreAsync()
            {
                await SaveEvent(context.Event);
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
                _eventHandledNotificationFlow.OnNewEventHandled(new EventNotifierContext
                {
                    Event = context.Event,
                    CurrentState = nextState,
                    OldState = context.NowState
                });
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
                try
                {
                    var re = new EventHandleFlowContext
                    {
                        NowState = _stateHolder.DeepCopy(State),
                        Event = item.Event,
                        TaskCompletionSource = item.TaskCompletionSource,
                    };
                    re.Event.Version = re.NowState.NextVersion;
                    re.EventContext = new EventContext(re.Event, re.NowState);
                    re.EventHandler = CreateHandler(re.EventContext);
                    return re;
                }
                catch (Exception e)
                {
                    item.TaskCompletionSource.SetException(e);
                    throw;
                }
            }

            async Task SaveEvent(IEvent @event)
            {
                try
                {
                    _logger.LogTrace("start to save event {@event}", @event);
                    await _eventSaver.SaveEventAsync(@event);
                }
                catch (EventSavingException e)
                {
                    _logger.LogError(e,
                        "failed to save event, please check event store {@event}", @event);
                    throw;
                }
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
            public TaskCompletionSource<int> TaskCompletionSource { get; set; } = null!;
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