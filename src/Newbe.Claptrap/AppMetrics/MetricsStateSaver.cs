using System.Threading.Tasks;

namespace Newbe.Claptrap.AppMetrics
{
    public class MetricsStateSaver : IStateSaver
    {
        private readonly IStateSaver _stateSaver;

        public MetricsStateSaver(
            IStateSaver stateSaver)
        {
            _stateSaver = stateSaver;
        }

        public IClaptrapIdentity Identity => _stateSaver.Identity;

        public async Task SaveAsync(IState state)
        {
            using var timer = ClaptrapMetrics.MeasureStateSaver(Identity);
            await _stateSaver.SaveAsync(state);
        }
    }
}