using System.Collections.Generic;

namespace Newbe.Claptrap.Localization.Modules
{
    public class ClaptrapApplicationModulesProvider : IClaptrapApplicationModulesProvider
    {
        public IEnumerable<IClaptrapApplicationModule> GetClaptrapApplicationModules()
        {
            yield return new LocalizationModule();
        }
    }
}