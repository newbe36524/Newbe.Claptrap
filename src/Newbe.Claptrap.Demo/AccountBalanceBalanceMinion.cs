using System.Threading.Tasks;
using Dapr.Actors.Runtime;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Dapr;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;
using Newbe.Claptrap.Demo.Models;
using static Newbe.Claptrap.Demo.Interfaces.Domain.Account.ClaptrapCodes.AccountCodes;

namespace Newbe.Claptrap.Demo
{
    [ClaptrapEventHandler(typeof(TransferAccountBalanceEventHandler), EventCodes.AccountBalanceChanged)]
    [Actor(TypeName = MinionCodes.BalanceMinion)]
    public class AccountBalanceBalanceMinion : ClaptrapBoxMinionActor<AccountStateData>,
        IAccountBalanceMinion
    {
        private readonly ILogger<AccountBalanceBalanceMinion> _logger;

        public AccountBalanceBalanceMinion(
            ActorHost actorHost,
            IClaptrapActorCommonService claptrapActorCommonService,
            ILogger<AccountBalanceBalanceMinion> logger) :
            base(actorHost, claptrapActorCommonService)
        {
            _logger = logger;
        }

        public Task<decimal> GetBalance()
        {
            var re = StateData.Balance;
            return Task.FromResult(re);
        }
    }
}