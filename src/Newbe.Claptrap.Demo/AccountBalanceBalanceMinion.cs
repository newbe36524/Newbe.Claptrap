using System.Collections.Generic;
using System.Linq;
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
    public class AccountBalanceBalanceMinion : ClaptrapBoxActor<AccountStateData>,
        IAccountBalanceMinion
    {
        private readonly ILogger<AccountBalanceBalanceMinion> _logger;
        private readonly IEventStringSerializer _eventStringSerializer;

        public AccountBalanceBalanceMinion(ActorHost actorService,
            IClaptrapActorCommonService claptrapActorCommonService,
            ILogger<AccountBalanceBalanceMinion> logger,
            IEventStringSerializer eventStringSerializer) : base(
            actorService, claptrapActorCommonService)
        {
            _logger = logger;
            _eventStringSerializer = eventStringSerializer;
        }

        public Task<decimal> GetBalance()
        {
            var re = StateData.Balance;
            return Task.FromResult(re);
        }

        public async Task MasterEventReceivedAsync(IEnumerable<IEvent> events)
        {
            foreach (var @event in events)
            {
                await Claptrap.HandleEventAsync(@event);
            }
        }

        public async Task MasterEventReceivedJsonAsync(IEnumerable<string> events)
        {
            var items = events.Select(_eventStringSerializer.Deserialize);
            foreach (var @event in items)
            {
                await Claptrap.HandleEventAsync(@event);
            }
        }

        public Task WakeAsync()
        {
            _logger.LogInformation("activated by master");
            return Task.CompletedTask;
        }
    }
}