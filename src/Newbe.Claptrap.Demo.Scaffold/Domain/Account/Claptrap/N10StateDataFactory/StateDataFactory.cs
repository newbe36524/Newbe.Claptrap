using System;
using System.Threading.Tasks;
using Newbe.Claptrap.Core;
using StateData = Newbe.Claptrap.Demo.Models.Domain.Account.AccountStateData;
namespace Newbe.Claptrap.Demo.Scaffold.Domain.Account.Claptrap.N10StateDataFactory
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
