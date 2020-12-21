using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Runtime;
using Newbe.Claptrap.Dapr;
using Newbe.Claptrap.Dapr.Core;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;
using Newbe.Claptrap.Demo.Models;
using static Newbe.Claptrap.Demo.Interfaces.Domain.Account.ClaptrapCodes.AccountCodes;

namespace Newbe.Claptrap.Demo
{
    // [ClaptrapMinionOptions(ActivateMinionsAtStart = true)]
    [ClaptrapEventHandler(typeof(TransferAccountBalanceEventHandler), EventCodes.AccountBalanceChanged)]
    public class AccountGrain : ClaptrapBoxActor<AccountStateData>, IAccount, IClaptrapActor
    {
        public AccountGrain(ActorHost actorService,
            IClaptrapActorCommonService claptrapGrainCommonService) : base(actorService,
            claptrapGrainCommonService)
        {
        }

        public async Task<decimal> TransferIn(decimal amount)
        {
            var accountBalanceChangeEventData = new AccountBalanceChangeEventData
            {
                Diff = +amount
            };
            var dataEvent = this.CreateEvent(accountBalanceChangeEventData);
            await Claptrap.HandleEventAsync(dataEvent);
            var re = await GetBalance();
            return re;
        }

        public Task<decimal> GetBalance()
        {
            var re = StateData.Balance;
            return Task.FromResult(re);
        }
    }
}