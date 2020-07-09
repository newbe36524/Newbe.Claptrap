using System.Collections.Generic;

namespace Newbe.Claptrap.Orleans.Modules
{
    public class ClaptrapApplicationModulesProvider : IClaptrapApplicationModulesProvider
    {
        public IEnumerable<IClaptrapApplicationModule> GetClaptrapApplicationModules()
        {
            yield return new ClaptrapOrleansModule();
        }
    }
}