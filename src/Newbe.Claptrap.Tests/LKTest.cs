using FluentAssertions;
using Newbe.Claptrap.Preview.Impl.Localization;
using Xunit;

namespace Newbe.Claptrap.Tests
{
    public class LKTest
    {
        [Fact]
        public void Init()
        {
            LK.Init();
            LK.L0002ClaptrapActor.L002LogStateSnapshotFound.Should().Be(LK.L0002ClaptrapActor.Prefix + "002");
        }
    }
}