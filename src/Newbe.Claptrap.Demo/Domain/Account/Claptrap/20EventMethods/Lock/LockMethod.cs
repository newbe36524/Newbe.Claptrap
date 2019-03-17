using System.Threading.Tasks;
using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Demo.Models.Domain.Account;

namespace Newbe.Claptrap.Demo.Domain.Account.Claptrap._20EventMethods.Lock
{
    public class LockMethod : ILockMethod
    {
        public Task<EventMethodResult<LockEventData>> Invoke(AccountStateData stateData)
        {
            if (stateData.Status != AccountStatus.Locked)
            {
                var result = EventMethodResult.Ok(new LockEventData());
                return Task.FromResult(result);
            }

            return Task.FromResult(EventMethodResult.None<LockEventData>());
        }
    }
}