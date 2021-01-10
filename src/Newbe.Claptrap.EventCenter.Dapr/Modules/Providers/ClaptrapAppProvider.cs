using System.Collections.Generic;
using Newbe.Claptrap.EventCenter.Dapr.Modules;

namespace Newbe.Claptrap.EventCenter.RabbitMQ.Modules.Providers
{
    public class ClaptrapAppProvider : IClaptrapAppProvider
    {
        public IEnumerable<IClaptrapAppModule> GetClaptrapApplicationModules()
        {
            yield return new DaprEventCenterModule();
        }
    }
}