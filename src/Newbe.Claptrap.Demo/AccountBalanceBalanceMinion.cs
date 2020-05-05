using System.Threading.Tasks;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;
using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Preview.Abstractions.Core;
using Newbe.Claptrap.Preview.Attributes;
using Newbe.Claptrap.Preview.Orleans;
using static Newbe.Claptrap.Demo.Interfaces.Domain.Account.ClaptrapCodes.AccountCodes;

namespace Newbe.Claptrap.Demo
{
    [ClaptrapStateInitialFactoryHandler]
    [ClaptrapEventHandler(typeof(TransferAccountBalanceEventHandler), EventCodes.AccountBalanceChanged)]
    public class AccountBalanceBalanceMinion : ClaptrapBoxGrain<AccountStateData>, IAccountBalanceMinion
    {
        public AccountBalanceBalanceMinion(IClaptrapGrainCommonService claptrapGrainCommonService)
            : base(claptrapGrainCommonService)
        {
        }

        public Task<decimal> GetBalance()
        {
            var re = StateData.Balance;
            return Task.FromResult(re);
        }

        public Task MasterCall(IEvent @event)
        {
            return Claptrap.HandleEventAsync(@event);
        }
    }
}