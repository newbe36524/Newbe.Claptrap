using System.Collections.Generic;

namespace Newbe.Claptrap.StorageProvider.Relational.Module.Providers
{
    public class ClaptrapApplicationModuleProvider : IClaptrapApplicationModuleProvider
    {
        public IEnumerable<IClaptrapApplicationModule> GetClaptrapApplicationModules()
        {
            yield return new ClaptrapRelationalStorageProviderModule();
        }
    }
}