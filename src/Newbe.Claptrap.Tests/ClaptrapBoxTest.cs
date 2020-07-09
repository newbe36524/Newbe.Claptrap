using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FluentAssertions;
using Newbe.Claptrap.Orleans;
using Newbe.Claptrap.Tests.QuickSetupTools;
using NUnit.Framework;

namespace Newbe.Claptrap.Tests
{
    public class ClaptrapBoxTest
    {
        [Test]
        public async Task MockClaptrapState()
        {
            using var mocker = AutoMock.GetStrict();
            const int balanceNow = 1000;

            mocker.Mock<IClaptrapAccessor>()
                .Setup(x => x.Claptrap.State.Data)
                .Returns(new AccountState
                {
                    Balance = balanceNow
                });

            var account = mocker.Create<Account>();
            var balance = await account.GetBalanceAsync();
            balance.Should().Be(balanceNow);
        }

        [Test]
        public async Task MockClaptrapStateInClaptrapBoxGrain()
        {
            using var mocker = AutoMock.GetStrict();
            const int balanceNow = 1000;

            mocker.Mock<IClaptrapGrainCommonService>()
                .Setup(x => x.ClaptrapAccessor.Claptrap.State.Data)
                .Returns(new AccountState
                {
                    Balance = balanceNow
                });

            var account = mocker.Create<AccountGrainBox>();
            var balance = await account.GetBalanceAsync();
            balance.Should().Be(balanceNow);
        }

        private class AccountGrainBox : ClaptrapBoxGrain<AccountState>
        {
            public AccountGrainBox(
                IClaptrapGrainCommonService claptrapGrainCommonService) : base(claptrapGrainCommonService)
            {
            }

            public Task<decimal> GetBalanceAsync()
            {
                return Task.FromResult(StateData.Balance);
            }
        }
    }
}