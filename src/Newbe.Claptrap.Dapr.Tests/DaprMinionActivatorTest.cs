using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.Moq;
using Dapr.Actors;
using Dapr.Actors.Client;
using Moq;
using Newbe.Claptrap.Design;
using NUnit.Framework;

namespace Newbe.Claptrap.Dapr.Tests
{
    public class DaprMinionActivatorTest
    {
        [Test]
        public async Task MinionFound()
        {
            var id = new ClaptrapIdentity("1", "2");
            var store = new ClaptrapDesignStore();
            var mainDesign = new ClaptrapDesign
            {
                ClaptrapTypeCode = id.TypeCode
            };
            store.AddOrReplace(mainDesign);
            var minionDesign = new ClaptrapDesign
            {
                ClaptrapTypeCode = "minion",
                ClaptrapMasterDesign = mainDesign
            };
            store.AddOrReplace(minionDesign);


            using var mocker = AutoMock.GetLoose(builder =>
            {
                builder.RegisterInstance(store)
                    .AsImplementedInterfaces()
                    .SingleInstance();
            });

            mocker.Mock<IActorProxyFactory>()
                .Setup(x => x.Create(new ActorId(id.Id), minionDesign.ClaptrapTypeCode, It.IsAny<ActorProxyOptions>()))
                .Verifiable();

            var daprMinionActivator = mocker.Create<DaprMinionActivator>();

            // act
            await daprMinionActivator.WakeAsync(id);
        }

        [Test]
        public async Task MinionNotFound()
        {
            var id = new ClaptrapIdentity("1", "2");
            var store = new ClaptrapDesignStore();
            var mainDesign = new ClaptrapDesign
            {
                ClaptrapTypeCode = id.TypeCode
            };
            store.AddOrReplace(mainDesign);


            using var mocker = AutoMock.GetLoose(builder =>
            {
                builder.RegisterInstance(store)
                    .AsImplementedInterfaces()
                    .SingleInstance();
            });

            var daprMinionActivator = mocker.Create<DaprMinionActivator>();

            // act
            await daprMinionActivator.WakeAsync(id);

            mocker.Mock<IActorProxyFactory>()
                .Verify(x => x.Create(It.IsAny<ActorId>(), It.IsAny<string>(), It.IsAny<ActorProxyOptions>()),
                    Times.Never);
        }
    }
}