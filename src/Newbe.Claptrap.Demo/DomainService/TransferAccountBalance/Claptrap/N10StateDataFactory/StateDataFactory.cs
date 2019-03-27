using System.Threading.Tasks;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Demo.Models.DomainService.TransferAccountBalance;

namespace Newbe.Claptrap.Demo.DomainService.TransferAccountBalance.Claptrap.N10StateDataFactory
{
    public class StateDataFactory
        : StateDataFactoryBase<TransferAccountBalanceStateData>
    {
        public StateDataFactory(IActorIdentity actorIdentity) : base(actorIdentity)
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