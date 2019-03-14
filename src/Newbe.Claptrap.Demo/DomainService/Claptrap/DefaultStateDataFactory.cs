using System.Threading.Tasks;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Demo.Interfaces.DomainService;

namespace Newbe.Claptrap.Demo.DomainService.Claptrap
{
    public class DefaultStateDataFactory
        : DefaultStateDataFactory<TransferAccountBalanceStateData>
    {
        public DefaultStateDataFactory(IActorIdentity actorIdentity) : base(actorIdentity)
        {
        }

        public override Task<TransferAccountBalanceStateData> Create()
        {
            return Task.FromResult(new TransferAccountBalanceStateData
            {
                Finished = false
            });
        }
    }
}