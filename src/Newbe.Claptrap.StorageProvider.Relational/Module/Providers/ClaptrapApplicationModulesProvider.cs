using System.Collections.Generic;

namespace Newbe.Claptrap.StorageProvider.Relational.Module.Providers
{
    public class ClaptrapApplicationModulesProvider : IClaptrapApplicationModulesProvider
    {
        public IEnumerable<IClaptrapApplicationModule> GetClaptrapApplicationModules()
        {
            yield return new ClaptrapRelationalStorageProviderModule();
        }
    }
}