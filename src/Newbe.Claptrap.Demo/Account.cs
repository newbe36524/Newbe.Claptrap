using System.Threading.Tasks;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;
using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Preview;
using Newbe.Claptrap.Preview.EventStore;
using Newbe.Claptrap.Preview.Orleans;

namespace Newbe.Claptrap.Demo
{
    [ClaptrapStateInitialFactoryHandler]
    [EventStore(EventStoreProvider.SQLite)]
    [StateStore(StateStoreProvider.SQLite)]
    [ClaptrapEventHandler(typeof(TransferAccountBalanceEventHandler), typeof(AccountBalanceChangeEventData))]
    public class Account : Claptrap<AccountStateData>, IAccount
    {
        public Account(IClaptrapGrainCommonService claptrapGrainCommonService)
            : base(claptrapGrainCommonService)
        {
        }

        public Task TransferIn(decimal amount, string uid)
        {
            var accountBalanceChangeEventData = new AccountBalanceChangeEventData
            {
                Diff = -amount
            };
            var dataEvent = this.CreateEvent(accountBalanceChangeEventData, uid);
            return Actor.HandleEvent(dataEvent);
        }

        public Task<decimal> GetBalance()
        {
            var re = StateData.Balance;
            return Task.FromResult(re);
        }
    }
}