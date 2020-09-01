using System.Collections.Generic;

namespace Newbe.Claptrap.StorageProvider.MongoDB.Module
{
    public class ClaptrapAppProvider : IClaptrapAppProvider
    {
        public IEnumerable<IClaptrapAppModule> GetClaptrapApplicationModules()
        {
            yield return new MongoDBStorageModule();
        }
    }
}