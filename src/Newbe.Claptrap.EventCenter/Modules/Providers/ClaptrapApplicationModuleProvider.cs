using System.Collections.Generic;

namespace Newbe.Claptrap.EventCenter.Modules.Providers
{
    public class ClaptrapApplicationModuleProvider : IClaptrapApplicationModuleProvider
    {
        public IEnumerable<IClaptrapApplicationModule> GetClaptrapApplicationModules()
        {
            yield return new EventCenterModule();
        }
    }
}