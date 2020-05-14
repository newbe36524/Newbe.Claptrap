using System.Collections.Generic;

namespace Newbe.Claptrap.StorageProvider.MySql.Module
{
    public class ClaptrapApplicationModuleProvider : IClaptrapApplicationModuleProvider
    {
        public IEnumerable<IClaptrapApplicationModule> GetClaptrapApplicationModules()
        {
            yield return new MySqlStorageModule();
        }
    }
}