using System.Collections.Generic;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.Module.Providers
{
    public class ClaptrapApplicationModuleProvider : IClaptrapApplicationModuleProvider
    {
        public IEnumerable<IClaptrapApplicationModule> GetClaptrapApplicationModules()
        {
            yield return new ClaptrapRelationDatabaseStorageProviderModule();
        }
    }
}