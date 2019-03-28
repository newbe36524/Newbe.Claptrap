using Newbe.Claptrap;
using System;
using System.Threading.Tasks;
using EventData = Newbe.Claptrap.ScaffoldGeneratorTest.TestEventDataType;
using StateData = Newbe.Claptrap.ScaffoldGeneratorTest.TestStateDataType;
namespace Claptrap.N20EventMethods
{
    public class IntReturnArgumentMethod : IIntReturnArgumentMethod
    {
        public Task<EventMethodResult<EventData, int>> Invoke(StateData stateData, string a, int b, TestEventDataType dataType)
        {
            throw new NotImplementedException();
        }
    }
}
