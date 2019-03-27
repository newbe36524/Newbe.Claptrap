using System;
using System.Threading.Tasks;
using EventData = Newbe.Claptrap.Demo.Models.Domain.Account.LockEventData;
using StateData = Newbe.Claptrap.Demo.Models.Domain.Account.AccountStateData;
namespace Newbe.Claptrap.Demo.Scaffold.Domain.Account.Claptrap.N20EventMethods.Lock
{
    public class LockMethod : ILockMethod
    {
        public Task<EventMethodResult<EventData>> Invoke(StateData stateData)
        {
            throw new NotImplementedException();
        }
    }
}
