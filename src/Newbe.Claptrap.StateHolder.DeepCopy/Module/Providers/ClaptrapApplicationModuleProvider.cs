using System.Collections.Generic;

namespace Newbe.Claptrap.Module.Providers
{
    public class ClaptrapApplicationModuleProvider : IClaptrapApplicationModuleProvider
    {
        public IEnumerable<IClaptrapApplicationModule> GetClaptrapApplicationModules()
        {
            yield return new DeepClonerStateHolderModule();
        }
    }
}