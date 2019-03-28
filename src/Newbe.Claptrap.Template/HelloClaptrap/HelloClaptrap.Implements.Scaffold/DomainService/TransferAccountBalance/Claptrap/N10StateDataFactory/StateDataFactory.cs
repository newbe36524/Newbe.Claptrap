using System;
using System.Threading.Tasks;
using Newbe.Claptrap;
using Newbe.Claptrap.Core;
using StateData = HelloClaptrap.Models.DomainService.TransferAccountBalance.TransferAccountBalanceStateData;
namespace HelloClaptrap.Implements.Scaffold.DomainService.TransferAccountBalance.Claptrap.N10StateDataFactory
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
