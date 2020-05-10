using System.Collections.Generic;
using System.Linq;

namespace Newbe.Claptrap
{
    public interface IClaptrapModuleProvider
    {
        IEnumerable<IClaptrapSharedModule> GetClaptrapSharedModules(IClaptrapIdentity identity)
        {
            return Enumerable.Empty<IClaptrapSharedModule>();
        }

        IEnumerable<IClaptrapMasterModule> GetClaptrapMasterClaptrapModules(IClaptrapIdentity identity)
        {
            return Enumerable.Empty<IClaptrapMasterModule>();
        }

        IEnumerable<IClaptrapMinionModule> GetClaptrapMinionModules(IClaptrapIdentity identity)
        {
            return Enumerable.Empty<IClaptrapMinionModule>();
        }
    }
}