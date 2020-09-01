using System.Collections.Generic;

namespace Newbe.Claptrap.StorageProvider.MySql.Module
{
    public class ClaptrapAppProvider : IClaptrapAppProvider
    {
        public IEnumerable<IClaptrapAppModule> GetClaptrapApplicationModules()
        {
            yield return new MySqlStorageModule();
        }
    }
}