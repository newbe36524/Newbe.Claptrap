using System.Threading.Tasks;
using Dapr.Actors.Runtime;
using Newbe.Claptrap.Dapr;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;
using Newbe.Claptrap.Demo.Models;
using static Newbe.Claptrap.Demo.Interfaces.Domain.Account.ClaptrapCodes.AccountCodes;

namespace Newbe.Claptrap.Demo
{
    [ClaptrapMinionOptions(ActivateMinionsAtStart = true)]
    [ClaptrapEventHandler(typeof(TransferAccountBalanceEventHandler), EventCodes.AccountBalanceChanged)]
    [Actor(TypeName = ClaptrapCode)]
    public class AccountActor : ClaptrapBoxActor<AccountStateData>, IAccount
    {
        public AccountActor(ActorHost actorService,
            IClaptrapActorCommonService claptrapActorCommonService) : base(actorService,
            claptrapActorCommonService)
        {
        }

        public async Task<decimal> TransferInAsync(decimal amount)
        {
            var accountBalanceChangeEventData = new AccountBalanceChangeEventData
            {
                Diff = +amount
            };
            var dataEvent = this.CreateEvent(accountBalanceChangeEventData);
            await Claptrap.HandleEventAsync(dataEvent);
            var re = await GetBalanceAsync();
            return re;
        }

        public Task<decimal> GetBalanceAsync()
        {
            var re = StateData.Balance;
            return Task.FromResult(re);
        }
    }
}