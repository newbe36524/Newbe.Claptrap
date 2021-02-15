using System.Threading.Tasks;

namespace Newbe.Claptrap.AppMetrics
{
    public class MetricsStateLoader : IStateLoader
    {
        private readonly IStateLoader _stateLoader;
        private readonly IClaptrapDesign _claptrapDesign;

        public MetricsStateLoader(
            IStateLoader stateLoader,
            IClaptrapDesign claptrapDesign)
        {
            _stateLoader = stateLoader;
            _claptrapDesign = claptrapDesign;
        }

        public IClaptrapIdentity Identity => _stateLoader.Identity;

        public async Task<IState?> GetStateSnapshotAsync()
        {
            using var a = ClaptrapActivitySource.Instance
                    .StartActivity(ClaptrapActivitySource.ActivityNames.LoadState)!
                .AddClaptrapTags(_claptrapDesign, _stateLoader.Identity);
            using var _ = ClaptrapMetrics.MeasureStateLoader(Identity);
            return await _stateLoader.GetStateSnapshotAsync();
        }
    }
}