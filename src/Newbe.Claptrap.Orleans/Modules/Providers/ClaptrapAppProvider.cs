using System.Collections.Generic;

namespace Newbe.Claptrap.Orleans.Modules
{
    public class ClaptrapAppProvider : IClaptrapAppProvider
    {
        public IEnumerable<IClaptrapAppModule> GetClaptrapApplicationModules()
        {
            yield return new ClaptrapOrleansModule();
        }

        public IEnumerable<IClaptrapDesignStoreConfigurator> GetClaptrapDesignStoreConfigurators()
        {
            yield return new OrleansClaptrapDesignStoreConfigurator();
        }
    }
}