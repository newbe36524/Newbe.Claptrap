using System.Threading.Tasks;

namespace Newbe.Claptrap.AppMetrics
{
    public class MetricsStateLoader : IStateLoader
    {
        private readonly IStateLoader _stateLoader;

        public MetricsStateLoader(
            IStateLoader stateLoader)
        {
            _stateLoader = stateLoader;
        }

        public IClaptrapIdentity Identity => _stateLoader.Identity;

        public async Task<IState?> GetStateSnapshotAsync()
        {
            using var _ = ClaptrapMetrics.MeasureStateLoader(Identity);
            return await _stateLoader.GetStateSnapshotAsync();
        }
    }
}