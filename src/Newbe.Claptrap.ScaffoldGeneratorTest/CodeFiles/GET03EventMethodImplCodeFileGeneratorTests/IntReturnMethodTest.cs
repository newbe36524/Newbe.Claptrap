using Newbe.Claptrap;
using System;
using System.Threading.Tasks;
using EventData = Newbe.Claptrap.ScaffoldGeneratorTest.TestEventDataType;
using StateData = Newbe.Claptrap.ScaffoldGeneratorTest.TestStateDataType;
namespace Claptrap.N20EventMethods
{
    public class IntReturnMethod : IIntReturnMethod
    {
        public Task<EventMethodResult<EventData, int>> Invoke(StateData stateData)
        {
            throw new NotImplementedException();
        }
    }
}
