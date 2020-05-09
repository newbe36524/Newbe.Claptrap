using FluentAssertions;
using Xunit;
using static Newbe.Claptrap.LK.L0001AutofacClaptrapBootstrapperBuilder;

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