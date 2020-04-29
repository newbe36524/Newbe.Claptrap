using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Preview.Abstractions.Components;
using Newbe.Claptrap.Preview.Abstractions.Core;
using Newbe.Claptrap.Preview.Abstractions.Exceptions;

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
        private readonly StateSavingOptions _stateSavingOptions;

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
            StateSavingOptions stateSavingOptions)
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
            _stateSavingOptions = stateSavingOptions;
            _incomingEventsSeq = new Subject<EventItem>();
            _nextStateSeq = new Subject<IState>();
        }

        public IState State { get; private set; } = null!;

        private IDisposable _eventHandleFlow = null!;
        private IDisposable _snapshotSavingFlow = null!;
        private readonly Subject<EventItem> _incomingEventsSeq;
        private readonly Subject<IState> _nextStateSeq;

        private async Task RestoreStateAsync()
        {
            var stateSnapshot = await _stateLoader.GetStateSnapshotAsync();
            if (stateSnapshot == null)
            {
                _logger.LogInformation("there is no state snapshot");
                var stateData = await _initialStateDataFactory.Create(_claptrapIdentity);
                State = new DataState(_claptrapIdentity, stateData, 0);
            }
            else
            {
                _logger.LogInformation("state snapshot found");
                State = stateSnapshot;
            }

            var eventsFromStore = GetEventFromVersion().ToObservable();
            ExceptionDispatchInfo? exceptionDispatchInfo = null;
            var restoreStateFlow = eventsFromStore
                .Select(evt =>
                {
                    var nowState = State;
                    var context = new StateRestoreFlowContext
                    {
                        NowState = nowState,
                        Event = evt,
                        EventContext = new EventContext(evt, nowState)
                    };
                    context.EventHandler = CreateHandler(context.EventContext);
                    return context;
                })
                .Select(context =>
                {
                    return Observable.FromAsync(async () =>
                        (await context.EventHandler.HandleEvent(context.EventContext), context));
                })
                .Concat()
                .Subscribe(
                    tuple =>
                    {
                        var (state, eventContext) = tuple;
                        _logger.LogDebug("start update to {@state}", state);
                        Debug.Assert(state.NextVersion == eventContext.Event.Version);
                        State = state;
                        State.IncreaseVersion();
                    },
                    ex =>
                    {
                        _logger.LogError(ex, "error when restore state from event store ");
                        exceptionDispatchInfo = ExceptionDispatchInfo.Capture(ex);
                    },
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

        public async Task ActivateAsync()
        {
            try
            {
                await RestoreStateAsync();
                CreateEventHandlingFLow();
                CreateStateSnapshotSavingFlow();
            }
            catch (Exception e)
            {
                throw new ActivateFailException(e, State.Identity);
            }
        }

        private void CreateStateSnapshotSavingFlow()
        {
            var dist = _nextStateSeq
                .Where(state => state != null)
                .DistinctUntilChanged(state => state.Version);

            IObservable<IObservable<IState>> window;
            var savingWindowTime = _stateSavingOptions.SavingWindowTime;
            var savingWindowVersionLimit = _stateSavingOptions.SavingWindowVersionLimit;
            if (savingWindowTime.HasValue && savingWindowVersionLimit.HasValue)
            {
                window = dist.Window(savingWindowTime.Value,
                    savingWindowVersionLimit.Value);
            }
            else if (savingWindowTime.HasValue)
            {
                window = dist.Window(savingWindowTime.Value);
            }
            else if (savingWindowVersionLimit.HasValue)
            {
                window = dist.Window(savingWindowVersionLimit.Value);
            }
            else
            {
                _logger.LogInformation(
                    "there is no state saving window specified, state will not be save by every saving window.");
                window = dist.Window(100)
                    .Where(x => false);
            }

            _snapshotSavingFlow = window
                .Select(observable =>
                {
                    return observable
                        .LastOrDefaultAsync()
                        .Select(state =>
                        {
                            return state != null
                                ? (state, Observable.FromAsync(() => SaveStateAsync(state)))
                                : (default, Observable.Empty<Unit>())!;
                        });
                })
                .Concat()
                .Subscribe(tuple =>
                {
                    var (state, seq) = tuple;
                    seq.Subscribe(_ =>
                        {
                            Debug.Assert(state != null, nameof(state) + " != null");
                            _logger.LogInformation("state snapshot save, version : {version}",
                                state.Version);
                        }, ex =>
                        {
                            Debug.Assert(state != null, nameof(state) + " != null");
                            _logger.LogError(
                                "thrown a exception when saving state snapshot, version : {version}",
                                state.Version);
                        })
                        .Dispose();
                });
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
                .Select(context =>
                {
                    return Observable.FromAsync(async () => (await SaveEvent(context.Event), context));
                })
                .Concat()
                .Select((tuple, i) =>
                {
                    var (eventSavingResult, context) = tuple;
                    return eventSavingResult switch
                    {
                        EventSavingResult.Success => (
                            Observable.FromAsync(() => context.EventHandler.HandleEvent(context.EventContext)),
                            context),
                        EventSavingResult.AlreadyAdded => (
                            Observable.Return(default(IState)),
                            context),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                })
                .Subscribe(tuple =>
                {
                    var (eventHandling, context) = tuple;
                    eventHandling.Subscribe(
                            nextState =>
                            {
                                if (nextState != null)
                                {
                                    _logger.LogInformation("event handled and updating state");
                                    _logger.LogDebug("start update to {@state}", nextState);
                                    State = nextState;
                                    State.IncreaseVersion();
                                    _nextStateSeq.OnNext(State);
                                    _logger.LogDebug("state version updated : {version}", State.Version);
                                }
                                else
                                {
                                    _logger.LogInformation("event already added, nothing would on going");
                                }

                                context.TaskCompletionSource.SetResult(0);
                            },
                            exception =>
                            {
                                State = context.NowState;
                                _logger.LogWarning(exception, "there is an exception when handle event : {@event} ",
                                    context.Event);
                                context.TaskCompletionSource.SetException(exception);
                            })
                        .Dispose();
                });


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

        public class StateRestoreFlowContext
        {
            public IState NowState { get; set; } = null!;
            public IEvent Event { get; set; } = null!;
            public EventContext EventContext { get; set; } = null!;
            public IEventHandler EventHandler { get; set; } = null!;
        }

        public async Task DeactivateAsync()
        {
            if (_stateSavingOptions.SaveWhenDeactivateAsync)
            {
                await SaveStateAsync(State);
            }

            _eventHandleFlow?.Dispose();
            _snapshotSavingFlow?.Dispose();
            _incomingEventsSeq?.Dispose();
            _nextStateSeq?.Dispose();
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