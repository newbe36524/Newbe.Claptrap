using System;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FluentAssertions;
using Newbe.Claptrap.Preview;
using Newbe.Claptrap.Preview.Abstractions.Metadata;
using Newbe.Claptrap.Preview.Impl;
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

            var actorIdentity = ClaptrapIdentity.Instance;
            mocker.Mock<IClaptrapDesignStoreAccessor>()
                .Setup(x => x.FindStateDataType(actorIdentity.TypeCode))
                .Returns(typeof(TestStateData));

            var handler = mocker.Create<DefaultInitialStateDataFactory>();
            var stateData = await handler.Create(actorIdentity);
            stateData.Should().NotBeNull();
            stateData.Should().BeOfType<TestStateData>();
        }
    }
}