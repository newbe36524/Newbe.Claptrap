using System.Threading.Tasks;
using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Demo.Models.EventData;

namespace Newbe.Claptrap.Demo.Impl.AccountImpl.Claptraps.EventMethods.LockImpl
{
    public interface ILockMethod
    {
        Task<EventMethodResult<LockEventData>> Invoke(AccountStateData stateData);
    }
}