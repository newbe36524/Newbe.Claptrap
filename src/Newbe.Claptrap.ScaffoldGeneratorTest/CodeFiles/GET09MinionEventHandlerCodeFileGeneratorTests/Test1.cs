using Newbe.Claptrap;
using Newbe.Claptrap.Core;
using System;
using System.Threading.Tasks;

using StateData = Newbe.Claptrap.ScaffoldGeneratorTest.TestStateDataType;
using EventData = Newbe.Claptrap.ScaffoldGeneratorTest.TestEventDataType;
namespace Minion.N30EventHandlersUpdaters
{
    public class TestStateDataTypeEventHandler : MinionEventHandlerBase<StateData, EventData>
    {
        public override Task HandleEventCore(StateData stateData, EventData eventData)
        {
            // TODO please add your code here and remove the exception
            throw new NotImplementedException();
        }
    }
}
