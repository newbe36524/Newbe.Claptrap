using System;
using System.Threading.Tasks;
using App.Metrics.Timer;
using Newbe.Claptrap.AppMetrics;

namespace Newbe.Claptrap.Core
{
    public class MetricsClaptrapLifetimeInterceptor : IClaptrapLifetimeInterceptor
    {
        private readonly IClaptrapIdentity _identity;

        public MetricsClaptrapLifetimeInterceptor(
            IClaptrapIdentity identity)
        {
            _identity = identity;
        }

        private TimerContext _eventHandlingTimer;

        public Task HandlingEventAsync(IEvent @event)
        {
            _eventHandlingTimer = ClaptrapMetrics.MeasureEventHandling(_identity, @event);
            return Task.CompletedTask;
        }

        public Task HandledEventAsync(IEvent @event)
        {
            _eventHandlingTimer.Dispose();
            return Task.CompletedTask;
        }

        public Task HandlingEventThrowExceptionAsync(IEvent @event, Exception ex)
        {
            _eventHandlingTimer.Dispose();
            return Task.CompletedTask;
        }

        private TimerContext _activationTimer;

        public Task ActivatingAsync()
        {
            _activationTimer = ClaptrapMetrics.MeasureActivation(_identity);
            return Task.CompletedTask;
        }

        public Task ActivatedAsync()
        {
            _activationTimer.Dispose();
            return Task.CompletedTask;
        }

        public Task ActivatingThrowExceptionAsync(Exception ex)
        {
            _activationTimer.Dispose();
            return Task.CompletedTask;
        }

        private TimerContext _deactivationTimer;

        public Task DeactivatingAsync()
        {
            _deactivationTimer = ClaptrapMetrics.MeasureDeactivation(_identity);
            return Task.CompletedTask;
        }

        public Task DeactivatedAsync()
        {
            _deactivationTimer.Dispose();
            return Task.CompletedTask;
        }

        public Task DeactivatingThrowExceptionAsync(Exception ex)
        {
            _deactivationTimer.Dispose();
            return Task.CompletedTask;
        }
    }
}