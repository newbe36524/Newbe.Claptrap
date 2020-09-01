using System.Collections.Generic;

namespace Newbe.Claptrap.DataSerializer.Json.Modules.Providers
{
    public class ClaptrapAppProvider : IClaptrapAppProvider
    {
        public IEnumerable<IClaptrapAppModule> GetClaptrapApplicationModules()
        {
            yield return new JsonSerializerModule();
        }
    }
}