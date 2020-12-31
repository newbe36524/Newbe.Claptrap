using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Newbe.Claptrap.StorageProvider.SQLite.Tests
{
    public class InterlockedTest
    {
        [Test]
        public async Task IncrementTest()
        {
            var target = new int[20];
            var index = 0;
            const int taskCount = 10;
            var tasks = Enumerable.Range(0, taskCount)
                .Select(async x =>
                {
                    var increment = Interlocked.Increment(ref index);
                    target[increment] = x;
                });
            await Task.WhenAll(tasks);
            target[0].Should().Be(0);
            target[1..(1 + taskCount)].Should().BeEquivalentTo(Enumerable.Range(0, taskCount));
        }

        [Test]
        public async Task IncrementTest2()
        {
            const int bufferSize = 20;
            var target = new int?[bufferSize];
            var index = -1;
            const int taskCount = 102;
            var readerEvent = new ManualResetEvent(false);
            var writerEvent = new ManualResetEvent(true);
            var tasks = Enumerable.Range(0, taskCount)
                .Select(async x =>
                {
                    var flag = true;
                    do
                    {
                        writerEvent.WaitOne();
                        var increment = Interlocked.Increment(ref index);
                        if (increment >= bufferSize)
                        {
                            writerEvent.Reset();
                            readerEvent.Set();
                        }
                        else
                        {
                            target[increment] = x;
                            readerEvent.Set();
                            flag = false;
                        }
                    } while (flag);
                });
            var list = new List<int>();
            Task.Run(() =>
            {
                while (true)
                {
                    readerEvent.WaitOne();
                    if (index != bufferSize - 1)
                    {
                        Thread.Sleep(TimeSpan.FromMilliseconds(50));
                    }

                    writerEvent.Reset();
                    var collection = target.Where(x => x.HasValue).Select(x => x.Value).ToArray();
                    Console.WriteLine(collection.Length);
                    list.AddRange(collection);
                    target = new int?[bufferSize];
                    Interlocked.Exchange(ref index, -1);
                    readerEvent.Reset();
                    writerEvent.Set();
                }
            });
            await Task.WhenAll(tasks);
            Thread.Sleep(TimeSpan.FromSeconds(1));
            list.OrderBy(x => x).Should().BeEquivalentTo(Enumerable.Range(0, taskCount));
        }
    }
}