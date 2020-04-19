using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.EventHandler;
using Newbe.Claptrap.EventStore;
using Newbe.Claptrap.StateStore;

namespace Newbe.Claptrap
{
    public class ReactiveActor : IActor
    {
        private readonly ILogger<ReactiveActor> _logger;
        private readonly IStateStore _stateStore;
        private readonly IEventStore _eventStore;
        private readonly IEventHandlerFactory _eventHandlerFactory;
        private readonly IStateHolder _stateHolder;

        public ReactiveActor(
            ILogger<ReactiveActor> logger,
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

        private IDisposable _disposable;
        private Subject<EventItem> _eventSeq;

        public async Task ActivateAsync()
        {
            var stateSnapshot = await _stateStore.GetStateSnapshot();
            State = stateSnapshot;
            var stateSeq = Observable.Return(stateSnapshot);
            _eventSeq = new Subject<EventItem>();
            _disposable = stateSeq
                .CombineLatest(_eventSeq, (state, item) => (oldState: _stateHolder.DeepCopy(state), item))
                .Where((tuple, i) =>
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
                })
                .Select((tuple, i) =>
                {
                    var (oldState, item) = tuple;
                    var eventContext = new EventContext(item.Event, oldState);
                    _logger.LogTrace("creating event handler");
                    var handler = _eventHandlerFactory.Create(eventContext);
                    _logger.LogTrace("created event handler : {handler}", handler);
                    var handlerSeq = Observable.FromAsync(() => handler.HandleEvent(eventContext));
                    return (item.Event, oldState, handlerSeq, item.TaskCompletionSource);
                })
                .Subscribe(tuple =>
                {
                    var (@event, oldState, handlerSeq, taskCompletionSource) = tuple;
                    handlerSeq.Subscribe(newState =>
                    {
                        _logger.LogInformation("event handled and updating state");
                        State = newState;
                        State.IncreaseVersion();
                        _logger.LogDebug("state version updated : {version}", State.Version);
                        taskCompletionSource.SetResult(0);
                    }, exception =>
                    {
                        State = oldState;
                        _logger.LogWarning(exception, "there is an exception when handle event : @{event} ", @event);
                        taskCompletionSource.SetException(exception);
                    }).Dispose();
                });


            await foreach (var @event in GetEventFromVersion())
            {
                _eventSeq.OnNext(new EventItem
                {
                    Event = @event,
                    TaskCompletionSource = new TaskCompletionSource<int>()
                });
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
        }

        public Task DeactivateAsync()
        {
            _disposable?.Dispose();
            return Task.CompletedTask;
        }

        public Task HandleEvent(IEvent @event)
        {
            var eventItem = new EventItem
            {
                Event = @event,
                TaskCompletionSource = new TaskCompletionSource<int>()
            };
            _eventSeq.OnNext(eventItem);
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