using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;
using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Orleans;
using static Newbe.Claptrap.Demo.Interfaces.Domain.Account.ClaptrapCodes.AccountCodes;

namespace Newbe.Claptrap.Demo
{
    [ClaptrapEventHandler(typeof(TransferAccountBalanceEventHandler), EventCodes.AccountBalanceChanged)]
    public class AccountBalanceBalanceMinion : ClaptrapBoxGrain<AccountStateData>,
        IAccountBalanceMinion
    {
        private readonly ILogger<AccountBalanceBalanceMinion> _logger;

        public AccountBalanceBalanceMinion(IClaptrapGrainCommonService claptrapGrainCommonService,
            ILogger<AccountBalanceBalanceMinion> logger)
            : base(claptrapGrainCommonService)
        {
            _logger = logger;
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

        public Task WakeAsync()
        {
            _logger.LogInformation("activated by master");
            return Task.CompletedTask;
        }
    }
}