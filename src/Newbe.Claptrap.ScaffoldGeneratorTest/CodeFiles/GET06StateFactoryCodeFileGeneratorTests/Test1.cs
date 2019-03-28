using Newbe.Claptrap;
using Newbe.Claptrap.Core;
using System;
using System.Threading.Tasks;
using StateData = Newbe.Claptrap.ScaffoldGeneratorTest.TestStateDataType;
namespace Claptrap.N10StateDataFactory
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
