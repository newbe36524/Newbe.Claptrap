using System.Collections.Generic;

namespace Newbe.Claptrap.EventCenter.RabbitMQ.Modules.Providers
{
    public class ClaptrapApplicationModulesProvider : IClaptrapApplicationModulesProvider
    {
        public IEnumerable<IClaptrapApplicationModule> GetClaptrapApplicationModules()
        {
            yield return new RabbitMQEventCenterModule();
        }
    }
}