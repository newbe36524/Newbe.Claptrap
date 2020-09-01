using System.Collections.Generic;
using System.Linq;

namespace Newbe.Claptrap
{
    /// <summary>
    /// Provider to provide some service in application wide
    /// </summary>
    public interface IClaptrapAppProvider
    {
        IEnumerable<IClaptrapAppModule> GetClaptrapApplicationModules()
        {
            return Enumerable.Empty<IClaptrapAppModule>();
        }

        IEnumerable<IClaptrapDesignStoreConfigurator> GetClaptrapDesignStoreConfigurators()
        {
            return Enumerable.Empty<IClaptrapDesignStoreConfigurator>();
        }
    }
}