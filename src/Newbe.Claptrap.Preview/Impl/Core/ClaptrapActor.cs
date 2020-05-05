using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Preview.Abstractions.Core;
using Newbe.Claptrap.Preview.Abstractions.Exceptions;
using Newbe.Claptrap.Preview.Impl.Localization;
using static Newbe.Claptrap.Preview.Impl.Localization.LK.L0002ClaptrapActor;

namespace Newbe.Claptrap.Preview.Impl
{
    public interface IClaptrapLifetimeInterceptor
    {
        Task ActivatingAsync();
        Task ActivatedAsync();
        Task ActivatingThrowExceptionAsync(Exception ex);

        Task DeactivatingAsync();
        Task DeactivatedAsync();
        Task DeactivatingThrowExceptionAsync(Exception ex);

        Task HandlingEventAsync(IEvent @event);
        Task HandledEventAsync(IEvent @event);
        Task HandlingEventThrowExceptionAsync(IEvent @event, Exception ex);
    }

    public class ClaptrapActorInterceptor : IClaptrap
    {
        private readonly IClaptrap _claptrap;
        private readonly IEnumerable<IClaptrapLifetimeInterceptor> _interceptors;

        public ClaptrapActorInterceptor(
            IClaptrap claptrap,
            IEnumerable<IClaptrapLifetimeInterceptor> interceptors)
        {
            _claptrap = claptrap;
            _interceptors = interceptors;
        }

        public IState State => _claptrap.State;

        public async Task ActivateAsync()
        {
            await RunInterceptors(x => x.ActivatingAsync());

            try
            {
                await _claptrap.ActivateAsync();
                await RunInterceptors(x => x.ActivatedAsync());
            }
            catch (Exception e)
            {
                await RunInterceptors(x => x.ActivatingThrowExceptionAsync(e));
                throw;
            }
        }

        public async Task DeactivateAsync()
        {
            await RunInterceptors(x => x.DeactivatingAsync());

            try
            {
                await _claptrap.DeactivateAsync();
                await RunInterceptors(x => x.DeactivatedAsync());
            }
            catch (Exception e)
            {
                await RunInterceptors(x => x.DeactivatingThrowExceptionAsync(e));
                throw;
            }
        }

        public async Task HandleEventAsync(IEvent @event)
        {
            await RunInterceptors(x => x.HandlingEventAsync(@event));

            try
            {
                await _claptrap.DeactivateAsync();
                await RunInterceptors(x => x.HandledEventAsync(@event));
            }
            catch (Exception e)
            {
                await RunInterceptors(x => x.HandlingEventThrowExceptionAsync(@event, e));
                throw;
            }
        }

        private async Task RunInterceptors(Func<IClaptrapLifetimeInterceptor, Task> action)
        {
            try
            {
                await Task.WhenAll(_interceptors.Select(action));
            }
            catch (Exception e)
            {
                // TODO logging
            }
        }
    }

    public class ClaptrapActor : IClaptrap
    {
        private readonly IClaptrapIdentity _claptrapIdentity;
        private readonly ILogger<ClaptrapActor> _logger;
        private readonly StateOptions _stateOptions;
        private readonly IStateAccessor _stateAccessor;
        private readonly IStateRestorer _stateRestorer;
        private readonly IEventHandlerFLow _eventHandlerFLow;
        private readonly IStateSavingFlow _stateSavingFlow;
        private readonly IEventHandledNotificationFlow _eventHandledNotificationFlow;
        private readonly IL _l;

        public ClaptrapActor(
            IClaptrapIdentity claptrapIdentity,
            ILogger<ClaptrapActor> logger,
            StateOptions stateOptions,
            IStateAccessor stateAccessor,
            IStateRestorer stateRestorer,
            IEventHandlerFLow eventHandlerFLow,
            IStateSavingFlow stateSavingFlow,
            IEventHandledNotificationFlow eventHandledNotificationFlow,
            IL l)
        {
            _claptrapIdentity = claptrapIdentity;
            _logger = logger;
            _stateOptions = stateOptions;
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
                throw new ActivateFailException(e, State.Identity);
            }
        }

        public async Task DeactivateAsync()
        {
            if (_stateOptions.SaveWhenDeactivateAsync)
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