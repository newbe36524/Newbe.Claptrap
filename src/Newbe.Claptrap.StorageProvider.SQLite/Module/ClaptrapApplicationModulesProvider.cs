using System.Collections.Generic;

namespace Newbe.Claptrap.StorageProvider.SQLite.Module
{
    public class ClaptrapApplicationModulesProvider : IClaptrapApplicationModulesProvider
    {
        public IEnumerable<IClaptrapApplicationModule> GetClaptrapApplicationModules()
        {
            yield return new SQLiteStorageSharedModule();
        }
    }
}