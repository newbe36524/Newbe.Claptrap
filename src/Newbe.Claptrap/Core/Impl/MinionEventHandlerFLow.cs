using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.Core.Impl
{
    public class MinionEventHandlerFLow : EventHandlerFLowBase
    {
        private readonly EventLoadingOptions _eventLoadingOptions;
        private readonly IEventLoader _eventLoader;

        public MinionEventHandlerFLow(IStateAccessor stateAccessor,
            IStateHolder stateHolder,
            IEventHandlerFactory eventHandlerFactory,
            IStateRestorer stateRestorer,
            IStateSavingFlow stateSavingFlow,
            StateRecoveryOptions stateRecoveryOptions,
            EventLoadingOptions eventLoadingOptions,
            IEventLoader eventLoader,
            ILogger<MinionEventHandlerFLow> logger) : base(stateAccessor,
            stateHolder,
            eventHandlerFactory,
            stateRestorer,
            stateSavingFlow,
            stateRecoveryOptions,
            logger)
        {
            _eventLoadingOptions = eventLoadingOptions;
            _eventLoader = eventLoader;
        }

        protected override async Task HandleCoreAsync(IEvent @event)
        {
            await foreach (var one in GetWaitingEvents(@event))
            {
                await HandleEventAndUpdateStateAsync(one);
            }

            async IAsyncEnumerable<IEvent> GetWaitingEvents(IEvent evt)
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

        protected override Task OnBeforeEventHandling(EventHandleFlowContext context)
        {
            if (context.Event.Version != context.NowState.NextVersion)
            {
                throw new VersionErrorException(context.NowState.Version, context.Event.Version);
            }

            return Task.CompletedTask;
        }

        protected override Task OnAfterEventHandled(EventHandleFlowContext context, IState newState)
        {
            return Task.CompletedTask;
        }
    }
}