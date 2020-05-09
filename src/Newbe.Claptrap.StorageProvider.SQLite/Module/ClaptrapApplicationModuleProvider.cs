using System.Collections.Generic;
using Newbe.Claptrap.Bootstrapper;

namespace Newbe.Claptrap.StorageProvider.SQLite.Module
{
    public class ClaptrapApplicationModuleProvider : IClaptrapApplicationModuleProvider
    {
        public IEnumerable<IClaptrapApplicationModule> GetClaptrapApplicationModules()
        {
            yield return new SQLiteStorageModule();
        }
    }
}