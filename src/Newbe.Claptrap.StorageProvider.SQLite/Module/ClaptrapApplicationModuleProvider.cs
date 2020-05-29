using System.Collections.Generic;

namespace Newbe.Claptrap.StorageProvider.SQLite.Module
{
    public class ClaptrapApplicationModuleProvider : IClaptrapApplicationModuleProvider
    {
        public IEnumerable<IClaptrapApplicationModule> GetClaptrapApplicationModules()
        {
            yield return new SQLiteStorageSharedModule();
        }
    }
}