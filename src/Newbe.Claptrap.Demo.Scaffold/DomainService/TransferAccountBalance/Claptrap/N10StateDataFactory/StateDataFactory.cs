using System;
using System.Threading.Tasks;
using Newbe.Claptrap.Core;
using StateData = Newbe.Claptrap.Demo.Models.DomainService.TransferAccountBalance.TransferAccountBalanceStateData;
namespace Newbe.Claptrap.Demo.Scaffold.DomainService.TransferAccountBalance.Claptrap.N10StateDataFactory
{
    public class StateDataFactory : StateDataFactoryBase<StateData>
    {
        public StateDataFactory(IActorIdentity actorIdentity) : base(actorIdentity)
        {
        }
        public override Task<StateData> Create()
        {
            throw new NotImplementedException();
        }
    }
}
