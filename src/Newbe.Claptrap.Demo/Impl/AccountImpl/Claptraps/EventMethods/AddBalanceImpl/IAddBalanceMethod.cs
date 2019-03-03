using System.Threading.Tasks;
using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Demo.Models.EventData;

namespace Newbe.Claptrap.Demo.Impl.AccountImpl.Claptraps.EventMethods.AddBalanceImpl
{
    public interface IAddBalanceMethod
    {
        Task<EventMethodResult<BalanceChangeEventData>> Invoke(AccountStateData stateData, decimal amount);
    }
}