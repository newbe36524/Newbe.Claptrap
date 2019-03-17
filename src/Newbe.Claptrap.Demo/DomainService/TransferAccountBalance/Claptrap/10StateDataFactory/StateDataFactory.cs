using System.Threading.Tasks;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Demo.Interfaces.DomainService.TransferAccountBalance;
using Newbe.Claptrap.Demo.Models.DomainService.TransferAccountBalance;

namespace Newbe.Claptrap.Demo.DomainService.TransferAccountBalance.Claptrap._10StateDataFactory
{
    public class StateDataFactory
        : StateDataFactory<TransferAccountBalanceStateData>
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