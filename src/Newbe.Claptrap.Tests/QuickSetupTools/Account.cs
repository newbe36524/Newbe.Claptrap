using System.Threading.Tasks;
using Newbe.Claptrap.Box;

namespace Newbe.Claptrap.Tests.QuickSetupTools
{
    [ClaptrapStateInitialFactoryHandler]
    [ClaptrapEventHandler(typeof(AccountBalanceChangeEventHandler), Codes.AccountBalanceChangeEvent)]
    public class Account : NormalClaptrapBox, IAccount
    {
        public new delegate Account Factory(IClaptrapIdentity identity);

        public Account(IClaptrapIdentity identity,
            IClaptrapFactory claptrapFactory) : base(identity,
            claptrapFactory)
        {
        }

        public Task<decimal> GetBalanceAsync()
        {
            var accountInfo = (AccountInfo) Claptrap.State.Data;
            return Task.FromResult(accountInfo.Balance);
        }

        public Task ChangeBalanceAsync(decimal diff)
        {
            var evt = new AccountBalanceChangeEvent
            {
                Diff = diff,
            };
            return Claptrap.HandleEventAsync(new DataEvent(Claptrap.State.Identity, Codes.AccountBalanceChangeEvent,
                evt));
        }

        public Task ActivateAsync()
        {
            return Claptrap.ActivateAsync();
        }

        public Task DeactivateAsync()
        {
            return Claptrap.DeactivateAsync();
        }
    }
}