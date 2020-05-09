using System.Collections.Generic;
using Newbe.Claptrap.Bootstrapper;

namespace Newbe.Claptrap.Orleans.Modules
{
    public class ClaptrapModuleProvider : IClaptrapModuleProvider
    {
        public IEnumerable<IClaptrapMasterModule> GetClaptrapMasterClaptrapModules(IClaptrapIdentity identity)
        {
            var re = new OrleansDirectlyEventCenterModule();
            yield return re;
        }
    }
}