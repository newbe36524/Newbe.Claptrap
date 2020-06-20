using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using static Newbe.Claptrap.LK.L0002ClaptrapActor;

namespace Newbe.Claptrap.Core
{
    public class ClaptrapActor : IClaptrap
    {
        private readonly IClaptrapIdentity _claptrapIdentity;
        private readonly ILogger<ClaptrapActor> _logger;
        private readonly StateSavingOptions _stateSavingOptions;
        private readonly IStateAccessor _stateAccessor;
        private readonly IStateRestorer _stateRestorer;
        private readonly IEventHandlerFLow _eventHandlerFLow;
        private readonly IStateSavingFlow _stateSavingFlow;
        private readonly IEventHandledNotificationFlow _eventHandledNotificationFlow;
        private readonly IL _l;

        public ClaptrapActor(
            IClaptrapIdentity claptrapIdentity,
            ILogger<ClaptrapActor> logger,
            StateSavingOptions stateSavingOptions,
            IStateAccessor stateAccessor,
            IStateRestorer stateRestorer,
            IEventHandlerFLow eventHandlerFLow,
            IStateSavingFlow stateSavingFlow,
            IEventHandledNotificationFlow eventHandledNotificationFlow,
            IL l)
        {
            _claptrapIdentity = claptrapIdentity;
            _logger = logger;
            _stateSavingOptions = stateSavingOptions;
            _stateAccessor = stateAccessor;
            _stateRestorer = stateRestorer;
            _eventHandlerFLow = eventHandlerFLow;
            _stateSavingFlow = stateSavingFlow;
            _eventHandledNotificationFlow = eventHandledNotificationFlow;
            _l = l;
        }

        public IState State => _stateAccessor.State;

        public async Task ActivateAsync()
        {
            try
            {
                _eventHandledNotificationFlow.Activate();
                await _stateRestorer.RestoreAsync();
                _stateSavingFlow.Activate();
                _eventHandlerFLow.Activate();
            }
            catch (Exception e)
            {
                _logger.LogError(e, _l[L001FailedToActivate], _claptrapIdentity);
                throw new ActivateFailException(e, _claptrapIdentity);
            }
        }

        public async Task DeactivateAsync()
        {
            if (_stateSavingOptions.SaveWhenDeactivateAsync)
            {
                await _stateSavingFlow.SaveStateAsync(State);
            }

            _eventHandledNotificationFlow.Deactivate();
            _stateSavingFlow.Deactivate();
            _eventHandlerFLow.Deactivate();
        }

        public Task HandleEventAsync(IEvent @event)
        {
            return _eventHandlerFLow.OnNewEventReceived(@event);
        }
    }
}