using System.Collections.Generic;

namespace Newbe.Claptrap.StorageProvider.PostgreSQL.Module
{
    public class ClaptrapAppProvider : IClaptrapAppProvider
    {
        public IEnumerable<IClaptrapAppModule> GetClaptrapApplicationModules()
        {
            yield return new PostgreSQLStorageModule();
        }
    }
}