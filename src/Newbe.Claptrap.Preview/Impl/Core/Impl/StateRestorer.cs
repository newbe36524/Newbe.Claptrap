using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Preview.Abstractions.Components;
using Newbe.Claptrap.Preview.Abstractions.Core;
using Newbe.Claptrap.Preview.Impl.Localization;
using static Newbe.Claptrap.Preview.Impl.Localization.LK.L0005StateRestorer;

namespace Newbe.Claptrap.Preview.Impl
{
    public class StateRestorer : IStateRestorer
    {
        private readonly IClaptrapIdentity _claptrapIdentity;
        private readonly IStateAccessor _stateAccessor;
        private readonly IInitialStateDataFactory _initialStateDataFactory;
        private readonly IStateLoader _stateLoader;
        private readonly IEventLoader _eventLoader;
        private readonly IEventHandlerFactory _eventHandlerFactory;
        private readonly IL _l;
        private readonly ILogger<StateRestorer> _logger;

        public StateRestorer(
            IClaptrapIdentity claptrapIdentity,
            IStateAccessor stateAccessor,
            IInitialStateDataFactory initialStateDataFactory,
            IStateLoader stateLoader,
            IEventLoader eventLoader,
            IEventHandlerFactory eventHandlerFactory,
            IL l,
            ILogger<StateRestorer> logger)
        {
            _claptrapIdentity = claptrapIdentity;
            _stateAccessor = stateAccessor;
            _initialStateDataFactory = initialStateDataFactory;
            _stateLoader = stateLoader;
            _eventLoader = eventLoader;
            _eventHandlerFactory = eventHandlerFactory;
            _l = l;
            _logger = logger;
        }

        private IState State
        {
            get => _stateAccessor.State;
            set => _stateAccessor.State = value;
        }

        public async Task RestoreAsync()
        {
            var stateSnapshot = await _stateLoader.GetStateSnapshotAsync();
            if (stateSnapshot == null)
            {
                _logger.LogInformation(_l[L001LogThereIsNoStateSnapshot]);
                var stateData = await _initialStateDataFactory.Create(_claptrapIdentity);
                State = new DataState(_claptrapIdentity, stateData, 0);
            }
            else
            {
                _logger.LogInformation(_l[L002LogStateSnapshotFound]);
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

        private IEventHandler CreateHandler(IEventContext eventContext)
        {
            _logger.LogTrace("creating event handler");
            var handler = _eventHandlerFactory.Create(eventContext);
            _logger.LogTrace("created event handler : {handler}", handler);
            return handler;
        }
    }
}