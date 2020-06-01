using System.Threading.Tasks;
using Newbe.Claptrap.Box;

namespace Newbe.Claptrap.Tests.QuickSetupTools
{
    [ClaptrapStateInitialFactoryHandler]
    [ClaptrapEventHandler(typeof(AccountBalanceChangeEventHandler), Codes.AccountBalanceChangeEvent)]
    public class AccountMinion : NormalClaptrapBox, IAccountMinion
    {
        public new delegate AccountMinion Factory(IClaptrapIdentity identity);

        public AccountMinion(IClaptrapIdentity identity, IClaptrapFactory claptrapFactory) : base(identity,
            claptrapFactory)
        {
        }

        public Task<decimal> GetBalanceAsync()
        {
            var re = (AccountInfo) Claptrap.State.Data;
            return Task.FromResult(re.Balance);
        }

        public Task ActivateAsync()
        {
            return Claptrap.ActivateAsync();
        }
    }
}