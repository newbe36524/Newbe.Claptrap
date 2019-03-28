using Newbe.Claptrap;
using System;
using System.Threading.Tasks;
using EventData = Newbe.Claptrap.ScaffoldGeneratorTest.TestEventDataType;
using StateData = Newbe.Claptrap.ScaffoldGeneratorTest.TestStateDataType;
namespace Claptrap.N20EventMethods
{
    public interface ITestTaskMethod
    {
        Task<EventMethodResult<EventData>> Invoke(StateData stateData);
    }
}
