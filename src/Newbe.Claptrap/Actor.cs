using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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
        private readonly ILogger<Actor> _logger;
        private readonly IStateStore _stateStore;
        private readonly IEventStore _eventStore;
        private readonly IEventHandlerFactory _eventHandlerFactory;
        private readonly IStateHolder _stateHolder;

        public Actor(
            ILogger<Actor> logger,
            IStateStore stateStore,
            IEventStore eventStore,
            IEventHandlerFactory eventHandlerFactory,
            IStateHolder stateHolder)
        {
            _logger = logger;
            _stateStore = stateStore;
            _eventStore = eventStore;
            _eventHandlerFactory = eventHandlerFactory;
            _stateHolder = stateHolder;
        }

        public IState State { get; private set; }

        private IDisposable _eventHandleFlow;
        private Subject<EventItem> _incomingEventsSeq;

        public async Task ActivateAsync()
        {
            var stateSnapshot = await _stateStore.GetStateSnapshot();
            State = stateSnapshot;
            var stateSeq = Observable.Return<Func<IState>>(() => State);

            var stateIsRestoreSuccess = true;
            var eventsFromStore = GetEventFromVersion().ToObservable();
            var restoreStateFlow = stateSeq
                .CombineLatest(eventsFromStore, (stateFunc, evt) => new EventContext(evt, stateFunc()))
                .Select((eventContext, i) =>
                {
                    var handler = CreateHandler(eventContext);
                    var handlerSeq =
                        Observable.FromAsync(async () => (await handler.HandleEvent(eventContext), eventContext));
                    return handlerSeq;
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
                        _logger.LogError("error when restore state from event store ");
                        stateIsRestoreSuccess = false;
                    },
                    () =>
                    {
                        _logger.LogDebug("success restore state from event store");
                        stateIsRestoreSuccess = true;
                    });
            restoreStateFlow.Dispose();
            if (!stateIsRestoreSuccess)
            {
                throw new ActivateFailException(stateSnapshot.Identity);
            }

            _incomingEventsSeq = new Subject<EventItem>();
            _eventHandleFlow = stateSeq
                .CombineLatest(_incomingEventsSeq,
                    (stateFunc, item) => (oldState: _stateHolder.DeepCopy(stateFunc()), item))
                .Where(FilterVersionErrorEvents)
                .Select((tuple, i) =>
                {
                    var (oldState, item) = tuple;
                    item.Event.Version = oldState.NextVersion;
                    var eventContext = new EventContext(item.Event, oldState);
                    var handler = CreateHandler(eventContext);
                    var handlerSeq = Observable.FromAsync(async () =>
                    {
                        var @event = eventContext.Event;
                        var eventSavingResult = await SaveEvent(@event);
                        if (eventSavingResult == EventSavingResult.Success)
                        {
                            var newState = await handler.HandleEvent(eventContext);
                            return (eventSavingResult, newState);
                        }

                        return (eventSavingResult, default)!;
                    });
                    return (item.Event, oldState, handlerSeq, item.TaskCompletionSource);
                })
                .Subscribe(tuple =>
                {
                    var (@event, oldState, handlerSeq, taskCompletionSource) = tuple;
                    handlerSeq.Subscribe((handlerResult) =>
                        {
                            var (eventSavingResult, newState) = handlerResult;
                            switch (eventSavingResult)
                            {
                                case EventSavingResult.Success:
                                    _logger.LogInformation("event handled and updating state");
                                    _logger.LogDebug("start update to @{state}", newState);
                                    State = newState;
                                    State.IncreaseVersion();
                                    _logger.LogDebug("state version updated : {version}", State.Version);
                                    taskCompletionSource.SetResult(0);
                                    break;
                                case EventSavingResult.AlreadyAdded:
                                    _logger.LogInformation("event already added, nothing would on going");
                                    taskCompletionSource.SetResult(0);
                                    break;
                                default:
                                    taskCompletionSource.SetException(new ArgumentOutOfRangeException());
                                    break;
                            }
                        }, exception =>
                        {
                            State = oldState;
                            _logger.LogWarning(exception, "there is an exception when handle event : @{event} ",
                                @event);
                            taskCompletionSource.SetException(exception);
                        })
                        .Dispose();
                });

            IEventHandler CreateHandler(IEventContext eventContext)
            {
                _logger.LogTrace("creating event handler");
                var handler = _eventHandlerFactory.Create(eventContext);
                _logger.LogTrace("created event handler : {handler}", handler);
                return handler;
            }

            async IAsyncEnumerable<IEvent> GetEventFromVersion()
            {
                Debug.Assert(stateSnapshot != null, nameof(stateSnapshot) + " != null");
                var nowVersion = stateSnapshot.Version;
                ulong stepVersion;
                const ulong step = 1000L;
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

            static bool FilterVersionErrorEvents((IState oldState, EventItem item) tuple, int i)
            {
                var (oldState, item) = tuple;
                if (item.Event.Version == 0)
                {
                    return true;
                }

                if (oldState.NextVersion == item.Event.Version)
                {
                    return true;
                }

                throw new VersionErrorException(oldState.Version, item.Event.Version);
            }

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