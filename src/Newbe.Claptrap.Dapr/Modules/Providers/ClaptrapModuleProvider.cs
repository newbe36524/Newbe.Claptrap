using System.Collections.Generic;
using System.Linq;

namespace Newbe.Claptrap.Dapr.Modules.Providers
{
    public class ClaptrapModuleProvider : IClaptrapModuleProvider
    {
        private readonly IClaptrapDesignStore _claptrapDesignStore;

        public ClaptrapModuleProvider(
            IClaptrapDesignStore claptrapDesignStore)
        {
            _claptrapDesignStore = claptrapDesignStore;
        }

        public IEnumerable<IClaptrapMasterModule> GetClaptrapMasterClaptrapModules(IClaptrapIdentity identity)
        {
            var design = _claptrapDesignStore.FindDesign(identity);
            if (design.ClaptrapOptions.EventCenterOptions.EventCenterType == EventCenterType.DaprClient)
            {
                var re = new DaprEventCenterModule();
                yield return re;
            }
        }
    }
}