using System.Collections.Generic;

namespace Newbe.Claptrap.DataSerializer.Json.Modules.Providers
{
    public class ClaptrapApplicationModulesProvider : IClaptrapApplicationModulesProvider
    {
        public IEnumerable<IClaptrapApplicationModule> GetClaptrapApplicationModules()
        {
            yield return new JsonSerializerModule();
        }
    }
}