using System;
using System.Threading.Tasks;
using Newbe.Claptrap.EventNotifier;
using NUnit.Framework;

namespace Newbe.Claptrap.Tests
{
    public class CompoundEventNotifierTest
    {
        [Test]
        public async Task Success()
        {
            using var mocker = AutoMockHelper.Create(builderAction: builder => { });
            var context = new EventNotifierContext();

            mocker.Mock<IEventNotifierHandler>()
                .Setup(x => x.Notify(context))
                .Returns(Task.CompletedTask);
            var notifier = mocker.Create<CompoundEventNotifier>();
            await notifier.Notify(context);
        }

        [Test]
        public void ExceptionAndContinue()
        {
            using var mocker = AutoMockHelper.Create(builderAction: builder => { });
            var context = new EventNotifierContext();

            mocker.Mock<IEventNotifierHandler>()
                .Setup(x => x.Notify(context))
                .Throws(new ArgumentNullException());
            var notifier = mocker.Create<CompoundEventNotifier>();
            Assert.ThrowsAsync<ArgumentNullException>(() => notifier.Notify(context));
        }
    }
}