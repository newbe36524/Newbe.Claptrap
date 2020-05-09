using System.Collections.Generic;
using Newbe.Claptrap.Bootstrapper;

namespace Newbe.Claptrap.Modules
{
    public class ClaptrapApplicationModuleProvider : IClaptrapApplicationModuleProvider
    {
        public IEnumerable<IClaptrapApplicationModule> GetClaptrapApplicationModules()
        {
            yield return new LocalizationModule();
        }
    }
}