using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Context;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.EventHandler;
using Newbe.Claptrap.EventStore;
using Newbe.Claptrap.StateStore;

namespace Newbe.Claptrap
{
    public class Actor : IActor
    {
        private readonly IActorIdentity _actorIdentity;
        private readonly ILogger<Actor> _logger;
        private readonly IInitialStateDataFactory _initialStateDataFactory;
        private readonly IStateStore _stateStore;
        private readonly IEventStore _eventStore;
        private readonly IEventHandlerFactory _eventHandlerFactory;
        private readonly IStateHolder _stateHolder;

        public Actor(
            IActorIdentity actorIdentity,
            ILogger<Actor> logger,
            IInitialStateDataFactory initialStateDataFactory,
            IStateStore stateStore,
            IEventStore eventStore,
            IEventHandlerFactory eventHandlerFactory,
            IStateHolder stateHolder)
        {
            _actorIdentity = actorIdentity;
            _logger = logger;
            _initialStateDataFactory = initialStateDataFactory;
            _stateStore = stateStore;
            _eventStore = eventStore;
            _eventHandlerFactory = eventHandlerFactory;
            _stateHolder = stateHolder;
            _stateSeq = Observable.Return<Func<IState>>(() => State);
            _incomingEventsSeq = new Subject<EventItem>();
        }

        public IState State { get; private set; }

        private IDisposable _eventHandleFlow;
        private readonly Subject<EventItem> _incomingEventsSeq;
        private readonly IObservable<Func<IState>> _stateSeq;

        private async Task RestoreState()
        {
            var stateSnapshot = await _stateStore.GetStateSnapshot();
            if (stateSnapshot == null)
            {
                _logger.LogInformation("there is no state snapshot");
                var stateData = await _initialStateDataFactory.Create(_actorIdentity);
                State = new DataState(_actorIdentity, stateData, 0);
            }
            else
            {
                _logger.LogInformation("state snapshot found");
                State = stateSnapshot;
            }

            var eventsFromStore = GetEventFromVersion().ToObservable();
            ExceptionDispatchInfo? exceptionDispatchInfo = null;
            var restoreStateFlow = _stateSeq
                .CombineLatest(eventsFromStore, (stateFunc, evt) =>
                {
                    var nowState = stateFunc();
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
                        _logger.LogDebug("start update to @{state}", state);
                        State = state;
                        Debug.Assert(state.NextVersion == eventContext.Event.Version);
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
                var nowVersion = State.Version;
                long stepVersion;
                const long step = 1000L;
                do
                {
                    stepVersion = nowVersion;
                    var left = stepVersion;
                    var right = stepVersion + step;
                    foreach (var @event in await _eventStore.GetEvents(left, right))
                    {
                        yield return @event;
                        nowVersion = @event.Version;
                    }
                } while (stepVersion != nowVersion);
            }
        }

        public async Task ActivateAsync()
        {
            try
            {
                await RestoreState();
                CreateEventHandlingFLow();
            }
            catch (Exception e)
            {
                throw new ActivateFailException(e, State.Identity);
            }
        }

        private void CreateEventHandlingFLow()
        {
            _eventHandleFlow = _stateSeq
                .CombineLatest(_incomingEventsSeq,
                    (stateFunc, item) =>
                    {
                        var context = new EventHandleFlowContext
                        {
                            NowState = _stateHolder.DeepCopy(stateFunc()),
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
                                    _logger.LogDebug("start update to @{state}", nextState);
                                    State = nextState;
                                    State.IncreaseVersion();
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
                                _logger.LogWarning(exception, "there is an exception when handle event : @{event} ",
                                    context.Event);
                                context.TaskCompletionSource.SetException(exception);
                            })
                        .Dispose();
                });


            async Task<EventSavingResult> SaveEvent(IEvent @event)
            {
                try
                {
                    _logger.LogTrace("start to save event @{event}", @event);
                    var eventSavingResult = await _eventStore.SaveEvent(@event);
                    switch (eventSavingResult)
                    {
                        case EventSavingResult.AlreadyAdded:
                            _logger.LogTrace("event AlreadyAdded before @{event}", @event);
                            break;
                        case EventSavingResult.Success:
                            _logger.LogTrace("event saved @{event}", @event);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    return eventSavingResult;
                }
                catch (EventSavingException e)
                {
                    _logger.LogError(e,
                        "failed to save event, please check event store @{event}", @event);
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

        private class EventHandleFlowContext
        {
            public IState NowState { get; set; }
            public IEvent Event { get; set; }
            public IEventContext EventContext { get; set; }
            public IEventHandler EventHandler { get; set; }
            public TaskCompletionSource<int> TaskCompletionSource { get; set; }
        }

        public class StateRestoreFlowContext
        {
            public IState NowState { get; set; }
            public IEvent Event { get; set; }
            public EventContext EventContext { get; set; }
            public IEventHandler EventHandler { get; set; }
        }

        public Task DeactivateAsync()
        {
            _eventHandleFlow?.Dispose();
            return Task.CompletedTask;
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