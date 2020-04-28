using System.Threading.Tasks;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;
using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.EventStore;
using Newbe.Claptrap.Orleans;

namespace Newbe.Claptrap.Demo
{
    [ClaptrapStateInitialFactoryHandler]
    [EventStore(EventStoreProvider.SQLite)]
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