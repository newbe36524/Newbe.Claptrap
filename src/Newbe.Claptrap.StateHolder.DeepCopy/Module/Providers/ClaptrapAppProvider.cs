using System.Collections.Generic;

namespace Newbe.Claptrap.Module.Providers
{
    public class ClaptrapAppProvider : IClaptrapAppProvider
    {
        public IEnumerable<IClaptrapAppModule> GetClaptrapApplicationModules()
        {
            yield return new DeepClonerStateHolderModule();
        }
    }
}