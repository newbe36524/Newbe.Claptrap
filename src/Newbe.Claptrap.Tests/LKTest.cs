using FluentAssertions;
using NUnit.Framework;
using static Newbe.Claptrap.LK.L0001AutofacClaptrapBootstrapperBuilder;

namespace Newbe.Claptrap.Tests
{
    public class LKTest
    {
        [Test]
        public void Init()
        {
            LK.Init();
            L001BuildException.Should().Be(Prefix + "001");
        }
    }
}