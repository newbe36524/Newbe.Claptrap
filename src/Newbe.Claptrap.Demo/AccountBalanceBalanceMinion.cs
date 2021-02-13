using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapr.Actors.Runtime;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Dapr;
using Newbe.Claptrap.Dapr.Core;
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
        private readonly IEventSerializer<EventJsonModel> _eventSerializer;

        public AccountBalanceBalanceMinion(IClaptrapActorCommonService claptrapActorCommonService,
            ILogger<AccountBalanceBalanceMinion> logger,
            IEventSerializer<EventJsonModel> eventSerializer) : base(claptrapActorCommonService)
        {
            _logger = logger;
            _eventSerializer = eventSerializer;
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

        public async Task MasterEventReceivedJsonAsync(IEnumerable<EventJsonModel> events)
        {
            var items = events.Select(_eventSerializer.Deserialize);
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