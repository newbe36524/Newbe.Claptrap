using System.Diagnostics;
using System.Threading.Tasks;
using App.Metrics.Timer;
using Newbe.Claptrap.AppMetrics;
using static Newbe.Claptrap.ClaptrapActivitySource.ActivityNames;

namespace Newbe.Claptrap.Core
{
    public class MetricsClaptrapLifetimeInterceptor : IClaptrapLifetimeInterceptor
    {
        private readonly IClaptrapIdentity _identity;
        private readonly IClaptrapDesign _claptrapDesign;

        public MetricsClaptrapLifetimeInterceptor(
            IClaptrapIdentity identity,
            IClaptrapDesign claptrapDesign)
        {
            _identity = identity;
            _claptrapDesign = claptrapDesign;
        }

        private TimerContext _eventHandlingTimer;
        private Activity? _handlingActivity;

        public Task HandlingEventAsync(IEvent @event)
        {
            _handlingActivity = ClaptrapActivitySource.Instance.StartActivity(HandleEvent)
                .AddClaptrapTags(_claptrapDesign, _identity);
            _eventHandlingTimer = ClaptrapMetrics.MeasureEventHandling(_identity, @event);
            return Task.CompletedTask;
        }

        public Task HandledEventFinallyAsync(IEvent @event)
        {
            _handlingActivity?.Dispose();
            _eventHandlingTimer.Dispose();
            return Task.CompletedTask;
        }

        private TimerContext _activationTimer;
        private Activity? _activatingActivity;

        public Task ActivatingAsync()
        {
            _activatingActivity = ClaptrapActivitySource.Instance.StartActivity(Activate)
                .AddClaptrapTags(_claptrapDesign, _identity);
            _activationTimer = ClaptrapMetrics.MeasureActivation(_identity);
            return Task.CompletedTask;
        }

        public Task ActivatedFinallyAsync()
        {
            _activatingActivity?.Dispose();
            _activationTimer.Dispose();
            return Task.CompletedTask;
        }

        private TimerContext _deactivationTimer;
        private Activity? _deactivatingActivity;

        public Task DeactivatingAsync()
        {
            _deactivatingActivity = ClaptrapActivitySource.Instance.StartActivity(Deactivate)
                .AddClaptrapTags(_claptrapDesign, _identity);
            _deactivationTimer = ClaptrapMetrics.MeasureDeactivation(_identity);
            return Task.CompletedTask;
        }

        public Task DeactivatedFinallyAsync()
        {
            _deactivatingActivity?.Dispose();
            _deactivationTimer.Dispose();
            return Task.CompletedTask;
        }
    }
}