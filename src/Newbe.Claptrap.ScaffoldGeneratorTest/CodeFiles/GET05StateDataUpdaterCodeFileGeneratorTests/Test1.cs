using Newbe.Claptrap;
using System;
using System.Threading.Tasks;
using StateData = Newbe.Claptrap.ScaffoldGeneratorTest.TestStateDataType;
using EventData = Newbe.Claptrap.ScaffoldGeneratorTest.TestEventDataType;
namespace Claptrap.N11StateDataUpdaters
{
    public class TestStateDataTypeUpdater : StateDataUpdaterBase<StateData, EventData>
    {
        public override void UpdateState(StateData stateData, EventData eventData)
        {
            throw new NotImplementedException();
        }
    }
}
