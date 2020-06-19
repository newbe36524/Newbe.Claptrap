using System.Threading.Tasks;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;
using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Orleans;
using static Newbe.Claptrap.Demo.Interfaces.Domain.Account.ClaptrapCodes.AccountCodes;

namespace Newbe.Claptrap.Demo
{
    [ClaptrapMinionOptions(ActivateMinionsAtStart = true)]
    [ClaptrapEventHandler(typeof(TransferAccountBalanceEventHandler), EventCodes.AccountBalanceChanged)]
    public class AccountGrain : ClaptrapBoxGrain<AccountStateData>, IAccount, IClaptrapGrain
    {
        public AccountGrain(IClaptrapGrainCommonService claptrapGrainCommonService)
            : base(claptrapGrainCommonService)
        {
        }

        public Task TransferIn(decimal amount, string uid)
        {
            var accountBalanceChangeEventData = new AccountBalanceChangeEventData
            {
                Diff = +amount
            };
            var dataEvent = this.CreateEvent(accountBalanceChangeEventData);
            return Claptrap.HandleEventAsync(dataEvent);
        }

        public Task<decimal> GetBalance()
        {
            var re = StateData.Balance;
            return Task.FromResult(re);
        }
    }
}