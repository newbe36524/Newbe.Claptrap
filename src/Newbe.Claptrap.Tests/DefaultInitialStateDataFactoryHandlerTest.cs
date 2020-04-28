using System;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FluentAssertions;
using Newbe.Claptrap.Preview;
using Newbe.Claptrap.Preview.Metadata;
using Xunit;

namespace Newbe.Claptrap.Tests
{
    public class DefaultInitialStateDataFactoryHandlerTest
    {
        [Fact]
        public async Task Create()
        {
            using var mocker = AutoMock.GetStrict();
            mocker.VerifyAll = true;

            var actorIdentity = ActorIdentity.Instance;
            mocker.Mock<IClaptrapRegistrationAccessor>()
                .Setup(x => x.FindStateDataType(actorIdentity.TypeCode))
                .Returns(typeof(TestStateData));

            var handler = mocker.Create<DefaultInitialStateDataFactoryHandler>();
            var stateData = await handler.Create(actorIdentity);
            stateData.Should().NotBeNull();
            stateData.Should().BeOfType<TestStateData>();
        }
    }
}