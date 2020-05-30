using System.Collections.Generic;

namespace Newbe.Claptrap.StorageProvider.PostgreSQL.Module
{
    public class ClaptrapApplicationModuleProvider : IClaptrapApplicationModuleProvider
    {
        public IEnumerable<IClaptrapApplicationModule> GetClaptrapApplicationModules()
        {
            yield return new PostgreSQLStorageModule();
        }
    }
}