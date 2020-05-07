using System.Threading.Tasks;
using Newbe.Claptrap.Preview.Abstractions;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Impl
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