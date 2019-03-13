using System.Threading.Tasks;
using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Demo.Models.EventData;

namespace Newbe.Claptrap.Demo.Account.Claptrap.EventMethods.AddBalance
{
    public interface IAddBalanceMethod
    {
        Task<EventMethodResult<BalanceChangeEventData>> Invoke(AccountStateData stateData, decimal amount);
    }
}