using System.Threading.Tasks;

namespace Newbe.Claptrap.AppMetrics
{
    public class MetricsStateSaver : IStateSaver
    {
        private readonly IStateSaver _stateSaver;
        private readonly IClaptrapDesign _claptrapDesign;

        public MetricsStateSaver(
            IStateSaver stateSaver,
            IClaptrapDesign claptrapDesign)
        {
            _stateSaver = stateSaver;
            _claptrapDesign = claptrapDesign;
        }

        public IClaptrapIdentity Identity => _stateSaver.Identity;

        public async Task SaveAsync(IState state)
        {
            // Do not save tracing save state since it is async flow.
            // If you keep it, it will make people wonder
            // using var a = ClaptrapActivitySource.Instance
            //         .StartActivity(ClaptrapActivitySource.ActivityNames.SaveState)!
            //     .AddClaptrapTags(_claptrapDesign, _stateSaver.Identity);
            using var _ = ClaptrapMetrics.MeasureStateSaver(Identity);
            await _stateSaver.SaveAsync(state);
        }
    }
}