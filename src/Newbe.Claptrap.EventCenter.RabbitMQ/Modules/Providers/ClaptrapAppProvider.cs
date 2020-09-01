using System.Collections.Generic;

namespace Newbe.Claptrap.EventCenter.RabbitMQ.Modules.Providers
{
    public class ClaptrapAppProvider : IClaptrapAppProvider
    {
        public IEnumerable<IClaptrapAppModule> GetClaptrapApplicationModules()
        {
            yield return new RabbitMQEventCenterModule();
        }
    }
}