using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Newbe.Claptrap.Tests
{
    public class AwaitableCompletionSourceTest
    {
        [Test]
        public async Task Completed()
        {
            var source = new AwaitableCompletionSource<int>();
            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                source.SetResult(1);
            });

            var i = await source;
            i.Should().Be(1);

            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                source.SetResult(2);
            });
            i = await source;
            i.Should().Be(2);

            source.SetException(new Exception());
            Assert.ThrowsAsync<Exception>(async () => await source);
        }

        [Test]
        public async Task BlockIfAwaitTwice()
        {
            var source = new AwaitableCompletionSource<int>();
            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                source.SetResult(1);
            });

            var i = await source;
            i.Should().Be(1);
            source.IsCompleted.Should().BeFalse();

            var delay = Task.Delay(TimeSpan.FromSeconds(1));
            var waitAny = await Task.WhenAny(delay, Task.Run(async () => await source));
            waitAny.Should().Be(delay);
        }
    }
}