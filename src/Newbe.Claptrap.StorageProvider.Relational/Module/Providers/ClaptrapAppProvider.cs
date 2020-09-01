using System.Collections.Generic;

namespace Newbe.Claptrap.StorageProvider.Relational.Module.Providers
{
    public class ClaptrapAppProvider : IClaptrapAppProvider
    {
        public IEnumerable<IClaptrapAppModule> GetClaptrapApplicationModules()
        {
            yield return new ClaptrapRelationalStorageProviderModule();
        }
    }
}