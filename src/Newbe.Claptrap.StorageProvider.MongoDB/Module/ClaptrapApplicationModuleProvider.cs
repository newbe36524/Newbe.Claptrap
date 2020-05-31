using System.Collections.Generic;

namespace Newbe.Claptrap.StorageProvider.MongoDB.Module
{
    public class ClaptrapApplicationModuleProvider : IClaptrapApplicationModuleProvider
    {
        public IEnumerable<IClaptrapApplicationModule> GetClaptrapApplicationModules()
        {
            yield return new MongoDBStorageModule();
        }
    }
}