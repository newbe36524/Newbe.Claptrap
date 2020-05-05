using System.Threading.Tasks;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;
using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Preview.Attributes;
using Newbe.Claptrap.Preview.Orleans;
using static Newbe.Claptrap.Demo.Interfaces.Domain.Account.ClaptrapCodes.AccountCodes;

namespace Newbe.Claptrap.Demo
{
    [ClaptrapStateInitialFactoryHandler]
    [ClaptrapEventHandler(typeof(TransferAccountBalanceEventHandler), EventCodes.AccountBalanceChanged)]
    public class Account : ClaptrapBoxGrain<AccountStateData>, IAccount
    {
        public Account(IClaptrapGrainCommonService claptrapGrainCommonService) 
            : base(claptrapGrainCommonService)
        {
        }

        public Task TransferIn(decimal amount, string uid)
        {
            var accountBalanceChangeEventData = new AccountBalanceChangeEventData
            {
                Diff = +amount
            };
            var dataEvent = this.CreateEvent(accountBalanceChangeEventData, uid);
            return Claptrap.HandleEventAsync(dataEvent);
        }

        public Task<decimal> GetBalance()
        {
            var re = StateData.Balance;
            return Task.FromResult(re);
        }
       
    }
}