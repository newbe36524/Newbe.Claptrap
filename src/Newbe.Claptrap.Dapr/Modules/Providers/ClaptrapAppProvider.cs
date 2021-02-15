using System.Collections.Generic;

namespace Newbe.Claptrap.Dapr.Modules.Providers
{
    public class ClaptrapAppProvider : IClaptrapAppProvider
    {
        public IEnumerable<IClaptrapAppModule> GetClaptrapApplicationModules()
        {
            yield return new ClaptrapDaprModule();
        }

        public IEnumerable<IClaptrapDesignStoreConfigurator> GetClaptrapDesignStoreConfigurators()
        {
            yield return new DaprClaptrapDesignStoreConfigurator();
        }
    }
}