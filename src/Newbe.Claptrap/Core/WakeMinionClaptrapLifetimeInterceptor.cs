using System.Threading.Tasks;

namespace Newbe.Claptrap.Core
{
    public class WakeMinionClaptrapLifetimeInterceptor : IClaptrapLifetimeInterceptor
    {
        private readonly IClaptrapIdentity _identity;
        private readonly IMinionActivator _minionActivator;

        public WakeMinionClaptrapLifetimeInterceptor(
            IClaptrapIdentity identity,
            IMinionActivator minionActivator)
        {
            _identity = identity;
            _minionActivator = minionActivator;
        }

        public Task ActivatedAsync()
        {
            return _minionActivator.WakeAsync(_identity);
        }
    }
}