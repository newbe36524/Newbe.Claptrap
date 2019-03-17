using System.Threading.Tasks;
using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Demo.Models.Domain.Account;

namespace Newbe.Claptrap.Demo.Domain.Account.Claptrap._20EventMethods.Lock
{
    public interface ILockMethod
    {
        Task<EventMethodResult<LockEventData>> Invoke(AccountStateData stateData);
    }
}