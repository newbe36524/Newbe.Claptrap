using Newbe.Claptrap;
using System.Threading.Tasks;
using EventData = Newbe.Claptrap.ScaffoldGeneratorTest.TestEventDataType;
using StateData = Newbe.Claptrap.ScaffoldGeneratorTest.TestStateDataType;
namespace Claptrap._20EventMethods
{
    public interface IIntReturnMethod
    {
        Task<EventMethodResult<EventData, int>> Invoke(StateData stateData);
    }
}
