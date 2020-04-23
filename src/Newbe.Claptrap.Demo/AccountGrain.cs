using System;
using System.Threading.Tasks;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;
using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Orleans;

namespace Newbe.Claptrap.Demo
{
    [ClaptrapState(typeof(AccountStateData))]
    public class AccountGrain : ClaptrapGrain<AccountStateData>, IAccount
    {
        public AccountGrain(IClaptrapGrainCommonService claptrapGrainCommonService)
            : base(claptrapGrainCommonService)
        {
        }

        public Task TransferIn(decimal amount, string uid)
        {
            var accountBalanceChangeEventData = new AccountBalanceChangeEventData
            {
                Diff = -amount
            };
            var dataEvent = this.CreateEvent(accountBalanceChangeEventData);
            return Actor.HandleEvent(dataEvent);
        }

        public Task<decimal> GetBalance()
        {
            var re = StateData.Balance;
            return Task.FromResult(re);
        }
    }
}