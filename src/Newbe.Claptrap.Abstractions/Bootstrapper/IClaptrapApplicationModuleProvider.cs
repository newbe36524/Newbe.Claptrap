using System.Collections.Generic;
using System.Linq;

namespace Newbe.Claptrap.Bootstrapper
{
    public interface IClaptrapApplicationModuleProvider
    {
        IEnumerable<IClaptrapApplicationModule> GetClaptrapApplicationModules()
        {
            return Enumerable.Empty<IClaptrapApplicationModule>();
        }
    }
}