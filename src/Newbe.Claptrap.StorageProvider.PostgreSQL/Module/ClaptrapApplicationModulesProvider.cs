using System.Collections.Generic;

namespace Newbe.Claptrap.StorageProvider.PostgreSQL.Module
{
    public class ClaptrapApplicationModulesProvider : IClaptrapApplicationModulesProvider
    {
        public IEnumerable<IClaptrapApplicationModule> GetClaptrapApplicationModules()
        {
            yield return new PostgreSQLStorageModule();
        }
    }
}