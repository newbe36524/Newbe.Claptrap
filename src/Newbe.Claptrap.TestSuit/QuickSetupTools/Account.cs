using System.Threading.Tasks;
using Newbe.Claptrap.Box;

namespace Newbe.Claptrap.TestSuit.QuickSetupTools
{
    [ClaptrapStateInitialFactoryHandler]
    [ClaptrapEventHandler(typeof(AccountBalanceChangeEventHandler), Codes.AccountBalanceChangeEvent)]
    [ClaptrapEventHandler(typeof(UnitEventHandler), UnitEvent.TypeCode)]
    public class Account : NormalClaptrapBox<AccountState>, IAccount
    {
        public delegate Account Factory(IClaptrapIdentity identity);

        public Account(IClaptrapIdentity identity,
            IClaptrapFactory claptrapFactory,
            IClaptrapAccessor claptrapAccessor) : base(identity,
            claptrapFactory,
            claptrapAccessor)
        {
        }

        public Task<decimal> GetBalanceAsync()
        {
            var accountInfo = StateData;
            return Task.FromResult(accountInfo.Balance);
        }

        public Task ChangeBalanceAsync(decimal diff)
        {
            var evt = new AccountBalanceChangeEvent
            {
                Diff = diff
            };
            var dataEvent = new DataEvent(Claptrap.State.Identity, Codes.AccountBalanceChangeEvent, evt);
            return Claptrap.HandleEventAsync(dataEvent);
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