using System.Collections.Generic;

namespace Newbe.Claptrap.Module.Providers
{
    public class ClaptrapApplicationModulesProvider : IClaptrapApplicationModulesProvider
    {
        public IEnumerable<IClaptrapApplicationModule> GetClaptrapApplicationModules()
        {
            yield return new DeepClonerStateHolderModule();
        }
    }
}