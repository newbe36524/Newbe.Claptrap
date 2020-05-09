using System;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FluentAssertions;
using Newbe.Claptrap.Design;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.Tests
{
    public class DefaultInitialStateDataFactoryHandlerTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public DefaultInitialStateDataFactoryHandlerTest(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Create()
        {
            using var mocker = AutoMockHelper.Create(_testOutputHelper);

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