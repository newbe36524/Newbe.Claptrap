using Newbe.Claptrap;
using System.Threading.Tasks;
using EventData = Newbe.Claptrap.Demo.Models.Domain.Account.LockEventData;
using StateData = Newbe.Claptrap.Demo.Models.Domain.Account.AccountStateData;
namespace Claptrap.N20EventMethods
{
    public class LockMethod : ILockMethod
    {
        public Task<EventMethodResult<EventData>> Invoke(StateData stateData)
        {
            throw new NotImplementedException();
        }
    }
}
