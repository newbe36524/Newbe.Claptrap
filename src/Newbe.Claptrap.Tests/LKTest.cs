using FluentAssertions;
using Newbe.Claptrap.Preview.Impl.Localization;
using Xunit;
using static Newbe.Claptrap.Preview.Impl.Localization.LK.L0001AutofacClaptrapBootstrapperBuilder;

namespace Newbe.Claptrap.Tests
{
    public class LKTest
    {
        [Fact]
        public void Init()
        {
            LK.Init();
            L001BuildException.Should().Be(Prefix + "001");
        }
    }
}