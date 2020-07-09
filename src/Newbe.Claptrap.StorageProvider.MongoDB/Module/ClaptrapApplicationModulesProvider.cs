using System.Collections.Generic;

namespace Newbe.Claptrap.StorageProvider.MongoDB.Module
{
    public class ClaptrapApplicationModulesProvider : IClaptrapApplicationModulesProvider
    {
        public IEnumerable<IClaptrapApplicationModule> GetClaptrapApplicationModules()
        {
            yield return new MongoDBStorageModule();
        }
    }
}