using System.Collections.Generic;

namespace Newbe.Claptrap.DataSerializer.Json.Modules.Providers
{
    public class ClaptrapApplicationModuleProvider : IClaptrapApplicationModuleProvider
    {
        public IEnumerable<IClaptrapApplicationModule> GetClaptrapApplicationModules()
        {
            yield return new JsonSerializerModule();
        }
    }
}