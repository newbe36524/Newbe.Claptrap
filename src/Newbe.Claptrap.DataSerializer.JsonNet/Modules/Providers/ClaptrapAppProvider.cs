using System.Collections.Generic;

namespace Newbe.Claptrap.DataSerializer.JsonNet.Modules.Providers
{
    public class ClaptrapAppProvider : IClaptrapAppProvider
    {
        public IEnumerable<IClaptrapAppModule> GetClaptrapApplicationModules()
        {
            yield return new JsonSerializerModule();
        }
    }
}