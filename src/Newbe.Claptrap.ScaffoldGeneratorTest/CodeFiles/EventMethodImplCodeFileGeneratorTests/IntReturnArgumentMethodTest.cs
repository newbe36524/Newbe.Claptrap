using Newbe.Claptrap;
using System.Threading.Tasks;
using EventData = Newbe.Claptrap.ScaffoldGeneratorTest.TestEventDataType;
using StateData = Newbe.Claptrap.ScaffoldGeneratorTest.TestStateDataType;
namespace Claptrap._20EventMethods
{
    public class IntReturnArgumentMethod : IIntReturnArgumentMethod
    {
        public Task<EventMethodResult<EventData, System.Int32>> Invoke(StateData stateData, System.String a, System.Int32 b, Newbe.Claptrap.ScaffoldGeneratorTest.TestEventDataType dataType)
        {
            throw new NotImplementedException();
        }
    }
}
