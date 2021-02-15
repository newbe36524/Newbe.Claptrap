using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Localization;

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
                _logger.LogTrace("Start to activate async");
                _eventHandledNotificationFlow.Activate();
                await _stateRestorer.RestoreAsync();
                _stateSavingFlow.Activate();
                _eventHandlerFLow.Activate();
                _logger.LogTrace("Activated");
            }
            catch (Exception e)
            {
                _logger.LogError(e, _l[LK.failed_to_activate_claptrap__identity_], _claptrapIdentity);
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