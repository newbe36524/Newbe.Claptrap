using System.Collections.Generic;
using System.Linq;

namespace Newbe.Claptrap
{
    public interface IClaptrapApplicationModuleProvider
    {
        IEnumerable<IClaptrapApplicationModule> GetClaptrapApplicationModules()
        {
            return Enumerable.Empty<IClaptrapApplicationModule>();
        }
    }
}