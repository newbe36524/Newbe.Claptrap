using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Preview.Abstractions.Components;
using Newbe.Claptrap.Preview.Abstractions.Core;
using Newbe.Claptrap.Preview.Abstractions.Exceptions;

namespace Newbe.Claptrap.Preview.Impl
{
    public class EventHandlerFLow : IEventHandlerFLow
    {
        private readonly IStateAccessor _stateAccessor;
        private readonly IEventSaver _eventSaver;
        private readonly IStateHolder _stateHolder;
        private readonly IEventHandlerFactory _eventHandlerFactory;
        private readonly IStateRestorer _stateRestorer;
        private readonly IStateSavingFlow _stateSavingFlow;
        private readonly IEventHandledNotificationFlow _eventHandledNotificationFlow;
        private readonly StateOptions _stateOptions;
        private readonly ILogger<EventHandlerFLow> _logger;

        private IDisposable _eventHandleFlow = null!;
        private readonly Subject<EventItem> _incomingEventsSeq;

        private IState State
        {
            get => _stateAccessor.State;
            set => _stateAccessor.State = value;
        }

        public EventHandlerFLow(
            IStateAccessor stateAccessor,
            IEventSaver eventSaver,
            IStateHolder stateHolder,
            IEventHandlerFactory eventHandlerFactory,
            IStateRestorer stateRestorer,
            IStateSavingFlow stateSavingFlow,
            IEventHandledNotificationFlow eventHandledNotificationFlow,
            StateOptions stateOptions,
            ILogger<EventHandlerFLow> logger)
        {
            _stateAccessor = stateAccessor;
            _eventSaver = eventSaver;
            _stateHolder = stateHolder;
            _eventHandlerFactory = eventHandlerFactory;
            _stateRestorer = stateRestorer;
            _stateSavingFlow = stateSavingFlow;
            _eventHandledNotificationFlow = eventHandledNotificationFlow;
            _stateOptions = stateOptions;
            _logger = logger;
            _incomingEventsSeq = new Subject<EventItem>();
        }

        public void Activate()
        {
            _eventHandleFlow = _incomingEventsSeq
                .Select(item =>
                {
                    try
                    {
                        var context = new EventHandleFlowContext
                        {
                            NowState = _stateHolder.DeepCopy(State),
                            Event = item.Event,
                            TaskCompletionSource = item.TaskCompletionSource,
                        };
                        context.Event.Version = context.NowState.NextVersion;
                        context.EventContext = new EventContext(context.Event, context.NowState);
                        context.EventHandler = CreateHandler(context.EventContext);
                        return context;
                    }
                    catch (Exception e)
                    {
                        item.TaskCompletionSource.SetException(e);
                        throw;
                    }
                })
                .Select(context => Observable.FromAsync(
                    async () =>
                    {
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
                            var eventSavingResult = await SaveEvent(context.Event);
                            switch (eventSavingResult)
                            {
                                case EventSavingResult.Success:
                                    var nextState = await context.EventHandler.HandleEvent(context.EventContext);
                                    _logger.LogInformation("event handled and updating state");
                                    _logger.LogDebug("start update to {@state}", nextState);
                                    State = nextState;
                                    State.IncreaseVersion();
                                    _stateSavingFlow.OnNewStateCreated(State);
                                    _eventHandledNotificationFlow.OnNewEventHandled(new EventHandledNotifierContext
                                    {
                                        Event = context.Event,
                                        CurrentState = nextState,
                                        EarlierState = context.NowState
                                    });
                                    _logger.LogDebug("state version updated : {version}", State.Version);
                                    break;
                                case EventSavingResult.AlreadyAdded:
                                    _logger.LogInformation("event already added, nothing would on going");
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }

                        async Task HandleException(Exception e)
                        {
                            _logger.LogWarning(e,
                                "there is an exception when handle event : {@event} . start to recover state as strategy : {strategy}",
                                context.Event,
                                _stateOptions.StateRecoveryStrategy);
                            switch (_stateOptions.StateRecoveryStrategy)
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
                    }))
                .Concat()
                .Subscribe();


            async Task<EventSavingResult> SaveEvent(IEvent @event)
            {
                try
                {
                    _logger.LogTrace("start to save event {@event}", @event);
                    var eventSavingResult = await _eventSaver.SaveEventAsync(@event);
                    switch (eventSavingResult)
                    {
                        case EventSavingResult.AlreadyAdded:
                            _logger.LogTrace("event AlreadyAdded before {@event}", @event);
                            break;
                        case EventSavingResult.Success:
                            _logger.LogTrace("event saved {@event}", @event);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    return eventSavingResult;
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
    }
}