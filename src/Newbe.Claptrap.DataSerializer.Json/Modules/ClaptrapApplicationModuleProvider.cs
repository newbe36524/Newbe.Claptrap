using System.Collections.Generic;
using Newbe.Claptrap.Bootstrapper;

namespace Newbe.Claptrap.DataSerializer.Json.Modules
{
    public class ClaptrapApplicationModuleProvider : IClaptrapApplicationModuleProvider
    {
        public IEnumerable<IClaptrapApplicationModule> GetClaptrapApplicationModules()
        {
            yield return new JsonSerializerModule();
        }
    }
}