using System.Collections.Generic;
using Newbe.Claptrap.Bootstrapper;

namespace Newbe.Claptrap.Orleans.Modules
{
    public class ClaptrapApplicationModuleProvider : IClaptrapApplicationModuleProvider
    {
        public IEnumerable<IClaptrapApplicationModule> GetClaptrapApplicationModules()
        {
            yield return new ClaptrapOrleansModule();
        }
    }
}