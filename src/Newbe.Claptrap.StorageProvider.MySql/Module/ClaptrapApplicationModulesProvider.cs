using System.Collections.Generic;

namespace Newbe.Claptrap.StorageProvider.MySql.Module
{
    public class ClaptrapApplicationModulesProvider : IClaptrapApplicationModulesProvider
    {
        public IEnumerable<IClaptrapApplicationModule> GetClaptrapApplicationModules()
        {
            yield return new MySqlStorageModule();
        }
    }
}