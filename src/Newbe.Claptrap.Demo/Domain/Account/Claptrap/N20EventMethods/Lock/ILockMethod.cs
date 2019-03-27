using System.Threading.Tasks;
using EventData = Newbe.Claptrap.Demo.Models.Domain.Account.LockEventData;
using StateData = Newbe.Claptrap.Demo.Models.Domain.Account.AccountStateData;

namespace Newbe.Claptrap.Demo.Domain.Account.Claptrap.N20EventMethods.Lock
{
    public interface ILockMethod
    {
        Task<EventMethodResult<EventData>> Invoke(StateData stateData);
    }
}