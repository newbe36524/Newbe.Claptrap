using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Preview.Abstractions.Components;
using Newbe.Claptrap.Preview.Abstractions.Core;
using Newbe.Claptrap.Preview.Abstractions.Exceptions;
using Newbe.Claptrap.Preview.Impl.Localization;
using LK = Newbe.Claptrap.Preview.Impl.Localization.LK.L0002ClaptrapActor;

namespace Newbe.Claptrap.Preview.Impl
{
    public class ClaptrapActor : IClaptrap
    {
        private readonly IClaptrapIdentity _claptrapIdentity;
        private readonly ILogger<ClaptrapActor> _logger;
        private readonly IInitialStateDataFactory _initialStateDataFactory;
        private readonly IStateSaver _stateSaver;
        private readonly IStateLoader _stateLoader;
        private readonly IEventSaver _eventSaver;
        private readonly IEventLoader _eventLoader;
        private readonly IEventHandlerFactory _eventHandlerFactory;
        private readonly IStateHolder _stateHolder;
        private readonly StateOptions _stateOptions;
        private readonly IEventHandledNotifier _eventHandledNotifier;
        private readonly IL _l;

        public ClaptrapActor(
            IClaptrapIdentity claptrapIdentity,
            ILogger<ClaptrapActor> logger,
            IInitialStateDataFactory initialStateDataFactory,
            IStateSaver stateSaver,
            IStateLoader stateLoader,
            IEventSaver eventSaver,
            IEventLoader eventLoader,
            IEventHandlerFactory eventHandlerFactory,
            IStateHolder stateHolder,
            StateOptions stateOptions,
            IEventHandledNotifier eventHandledNotifier,
            IL l)
        {
            _claptrapIdentity = claptrapIdentity;
            _logger = logger;
            _initialStateDataFactory = initialStateDataFactory;
            _stateSaver = stateSaver;
            _stateLoader = stateLoader;
            _eventSaver = eventSaver;
            _eventLoader = eventLoader;
            _eventHandlerFactory = eventHandlerFactory;
            _stateHolder = stateHolder;
            _stateOptions = stateOptions;
            _eventHandledNotifier = eventHandledNotifier;
            _l = l;
            _incomingEventsSeq = new Subject<EventItem>();
            _nextStateSeq = new Subject<IState>();
            _eventHandledNotifierContextSeq = new Subject<IEventHandledNotifierContext>();
        }

        public IState State { get; set; } = null!;

        private IDisposable _eventHandleFlow = null!;
        private IDisposable _snapshotSavingFlow = null!;
        private IDisposable _eventHandledNotifierFlow = null!;
        private readonly Subject<EventItem> _incomingEventsSeq;
        private readonly Subject<IState> _nextStateSeq;
        private readonly Subject<IEventHandledNotifierContext> _eventHandledNotifierContextSeq;

        public async Task ActivateAsync()
        {
            try
            {
                CreateEventHandledNotifierFlow();
                await RestoreStateAsync();
                CreateFlows();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "failed to activate claptrap {identity}", _claptrapIdentity);
                throw new ActivateFailException(e, State.Identity);
            }
        }

        public void CreateFlows()
        {
            CreateEventHandlingFLow();
            CreateStateSnapshotSavingFlow();
        }

        public async Task RestoreStateAsync()
        {
            var stateSnapshot = await _stateLoader.GetStateSnapshotAsync();
            if (stateSnapshot == null)
            {
                var localizedString = _l[LK.L001LogThereIsNoStateSnapshot];
                _logger.LogInformation(localizedString);
                var stateData = await _initialStateDataFactory.Create(_claptrapIdentity);
                State = new DataState(_claptrapIdentity, stateData, 0);
            }
            else
            {
                var localizedString = _l[LK.L002LogStateSnapshotFound];
                _logger.LogInformation(localizedString);
                State = stateSnapshot;
            }

            var eventsFromStore = GetEventFromVersion().ToObservable();
            ExceptionDispatchInfo? exceptionDispatchInfo = null;
            var restoreStateFlow = eventsFromStore
                .Select(evt => Observable.FromAsync(
                    async () =>
                    {
                        try
                        {
                            var nowState = State;
                            var eventContext = new EventContext(evt, nowState);
                            var handler = CreateHandler(eventContext);
                            var newState = await handler.HandleEvent(eventContext);
                            _logger.LogDebug("start update to {@state}", newState);
                            Debug.Assert(newState.NextVersion == eventContext.Event.Version);
                            State = newState;
                            State.IncreaseVersion();
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, "error when restore state from event store ");
                            exceptionDispatchInfo = ExceptionDispatchInfo.Capture(e);
                            throw;
                        }
                    }
                ))
                .Concat()
                .Subscribe(
                    _ => { },
                    ex => { },
                    () => { _logger.LogDebug("success restore state from event store"); });
            restoreStateFlow.Dispose();
            exceptionDispatchInfo?.Throw();

            async IAsyncEnumerable<IEvent> GetEventFromVersion()
            {
                var startVersion = State.NextVersion;
                const long step = 1000L;

                var left = startVersion;
                var right = startVersion + step;
                var any = true;
                while (any)
                {
                    any = false;
                    foreach (var @event in await _eventLoader.GetEventsAsync(left, right))
                    {
                        any = true;
                        yield return @event;
                    }

                    left = right;
                    right += step;
                }
            }
        }

        private void CreateEventHandledNotifierFlow()
        {
            _eventHandledNotifierFlow = _eventHandledNotifierContextSeq
                .Select(context => Observable.FromAsync(async () =>
                {
                    var version = context.Event.Version;
                    try
                    {
                        await _eventHandledNotifier.Notify(context);
                        _logger.LogDebug(_l[LK.L004SuccessToNotify], version);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, _l[LK.L003FailToNotify], version);
                    }
                }))
                .Concat()
                .Subscribe();
        }

        private void CreateStateSnapshotSavingFlow()
        {
            var dist = _nextStateSeq
                .Where(state => state != null)
                .DistinctUntilChanged(state => state.Version);

            IObservable<IList<IState>> stateBuffer;
            var savingWindowTime = _stateOptions.SavingWindowTime;
            var savingWindowVersionLimit = _stateOptions.SavingWindowVersionLimit;
            if (savingWindowTime.HasValue && savingWindowVersionLimit.HasValue)
            {
                stateBuffer = dist.Buffer(savingWindowTime.Value,
                    savingWindowVersionLimit.Value);
            }
            else if (savingWindowTime.HasValue)
            {
                stateBuffer = dist.Buffer(savingWindowTime.Value);
            }
            else if (savingWindowVersionLimit.HasValue)
            {
                stateBuffer = dist.Buffer(savingWindowVersionLimit.Value);
            }
            else
            {
                _logger.LogInformation(
                    "there is no state saving window specified, state will not be save by every saving window.");
                stateBuffer = dist.Buffer(100)
                    .Where(x => false);
            }

            _snapshotSavingFlow = stateBuffer
                .Where(x => x.Count > 0)
                .Select(list =>
                {
                    var latestState = list.Last();
                    return Observable.FromAsync(async () =>
                    {
                        try
                        {
                            await SaveStateAsync(latestState);
                            _logger.LogInformation("state snapshot save, version : {version}",
                                latestState.Version);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(
                                e,
                                "thrown a exception when saving state snapshot, version : {version}",
                                latestState.Version);
                        }
                    });
                })
                .Concat()
                .Subscribe();
        }

        private void CreateEventHandlingFLow()
        {
            _eventHandleFlow = _incomingEventsSeq
                .Select(item =>
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
                                    _nextStateSeq.OnNext(State);
                                    _eventHandledNotifierContextSeq.OnNext(new EventHandledNotifierContext
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
                                    await RestoreStateAsync();
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

        private IEventHandler CreateHandler(IEventContext eventContext)
        {
            _logger.LogTrace("creating event handler");
            var handler = _eventHandlerFactory.Create(eventContext);
            _logger.LogTrace("created event handler : {handler}", handler);
            return handler;
        }

        private async Task SaveStateAsync(IState state)
        {
            _logger.LogDebug("start to save state");
            await _stateSaver.SaveAsync(state);
            _logger.LogInformation("state save success");
        }

        private class EventHandleFlowContext
        {
            public IState NowState { get; set; } = null!;
            public IEvent Event { get; set; } = null!;
            public IEventContext EventContext { get; set; } = null!;
            public IEventHandler EventHandler { get; set; } = null!;
            public TaskCompletionSource<int> TaskCompletionSource { get; set; } = null!;
        }

        public async Task DeactivateAsync()
        {
            if (_stateOptions.SaveWhenDeactivateAsync)
            {
                await SaveStateAsync(State);
            }

            _eventHandleFlow?.Dispose();
            _snapshotSavingFlow?.Dispose();
            _incomingEventsSeq?.Dispose();
            _nextStateSeq?.Dispose();
            _eventHandledNotifierFlow?.Dispose();
            _eventHandledNotifierContextSeq.Dispose();
        }

        public Task HandleEvent(IEvent @event)
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

        private struct EventItem
        {
            public IEvent Event { get; set; }
            public TaskCompletionSource<int> TaskCompletionSource { get; set; }
        }
    }
}