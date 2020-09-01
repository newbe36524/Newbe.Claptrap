using System.Collections.Generic;

namespace Newbe.Claptrap.StorageProvider.SQLite.Module
{
    public class ClaptrapAppProvider : IClaptrapAppProvider
    {
        public IEnumerable<IClaptrapAppModule> GetClaptrapApplicationModules()
        {
            yield return new SQLiteStorageSharedModule();
        }
    }
}