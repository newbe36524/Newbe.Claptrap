using System.Collections.Generic;
using System.Linq;

namespace Newbe.Claptrap
{
    /// <summary>
    /// Provider to provider some service in application wide
    /// </summary>
    public interface IClaptrapApplicationModulesProvider
    {
        IEnumerable<IClaptrapApplicationModule> GetClaptrapApplicationModules()
        {
            return Enumerable.Empty<IClaptrapApplicationModule>();
        }
    }
}