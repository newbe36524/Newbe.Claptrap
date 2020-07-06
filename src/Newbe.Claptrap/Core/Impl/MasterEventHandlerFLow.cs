using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.Core.Impl
{
    public class MasterEventHandlerFLow : EventHandlerFLowBase
    {
        private readonly IEventSaver _eventSaver;
        private readonly IEventHandledNotificationFlow _eventHandledNotificationFlow;
        private readonly ILogger<MasterEventHandlerFLow> _logger;

        public MasterEventHandlerFLow(IStateAccessor stateAccessor,
            IStateHolder stateHolder,
            IEventHandlerFactory eventHandlerFactory,
            IStateRestorer stateRestorer,
            IStateSavingFlow stateSavingFlow,
            StateRecoveryOptions stateRecoveryOptions,
            IEventSaver eventSaver,
            IEventHandledNotificationFlow eventHandledNotificationFlow,
            ILogger<MasterEventHandlerFLow> logger) : base(stateAccessor,
            stateHolder,
            eventHandlerFactory,
            stateRestorer,
            stateSavingFlow,
            stateRecoveryOptions,
            logger)
        {
            _eventSaver = eventSaver;
            _eventHandledNotificationFlow = eventHandledNotificationFlow;
            _logger = logger;
        }

        protected override Task HandleCoreAsync(IEvent @event)
        {
            return HandleEventAndUpdateStateAsync(@event);
        }

        protected override Task OnBeforeEventHandling(EventHandleFlowContext context)
        {
            context.Event.Version = context.NowState.NextVersion;
            return SaveEvent(context.Event);

            async Task SaveEvent(IEvent evt)
            {
                try
                {
                    _logger.LogTrace("start to save event {@event}", evt);
                    await _eventSaver.SaveEventAsync(evt);
                }
                catch (EventSavingException e)
                {
                    _logger.LogError(e,
                        "failed to save event, please check event store {@event}", evt);
                    throw;
                }
            }
        }

        protected override Task OnAfterEventHandled(EventHandleFlowContext context,
            IState newState)
        {
            _eventHandledNotificationFlow.OnNewEventHandled(new EventNotifierContext
            {
                Event = context.Event,
                CurrentState = newState,
                OldState = context.NowState
            });
            return Task.CompletedTask;
        }
    }
}