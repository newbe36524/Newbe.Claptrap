using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Newbe.Claptrap.Tests
{
    public class DefaultInitialStateDataFactoryHandlerTest
    {
        [Test]
        public async Task Create()
        {
            using var mocker = AutoMockHelper.Create();

            var actorIdentity = TestClaptrapIdentity.Instance;
            mocker.Mock<IClaptrapDesignStore>()
                .Setup(x => x.FindDesign(actorIdentity))
                .Returns(new ClaptrapDesign
                {
                    StateDataType = typeof(TestStateData)
                });

            var handler = mocker.Create<DefaultInitialStateDataFactory>();
            var stateData = await handler.Create(actorIdentity);
            stateData.Should().NotBeNull();
            stateData.Should().BeOfType<TestStateData>();
        }
    }
}