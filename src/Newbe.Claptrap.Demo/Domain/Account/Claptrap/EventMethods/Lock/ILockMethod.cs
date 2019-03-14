using System.Threading.Tasks;
using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Demo.Models.EventData;

namespace Newbe.Claptrap.Demo.Domain.Account.Claptrap.EventMethods.Lock
{
    public interface ILockMethod
    {
        Task<EventMethodResult<LockEventData>> Invoke(AccountStateData stateData);
    }
}