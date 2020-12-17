using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Newbe.Claptrap.Tests
{
    public class TaskWhenAllTest
    {
        [Test]
        public async Task WhenAllCompleted()
        {
            var tasks = Enumerable.Range(0, 10_000)
                .Select(x => Task.Run(() => Console.Write(1)));

            await tasks.WhenAllComplete();
        }

        [Test]
        public void WhenAllCompletedWithError()
        {
            Assert.ThrowsAsync<AggregateException>(() => GetTasks(10_000).WhenAllComplete());

            IEnumerable<Task> GetTasks(int count)
            {
                for (int i = 0; i < count - 1; i++)
                {
                    yield return Task.CompletedTask;
                }

                yield return Task.FromException(new Exception());
            }
        }

        [Test]
        public void WhenAllCompletedWithCancel()
        {
            Assert.ThrowsAsync<TaskCanceledException>(async () => await GetTasks(10_000).WhenAllComplete());

            IEnumerable<Task> GetTasks(int count)
            {
                for (int i = 0; i < count - 1; i++)
                {
                    yield return Task.CompletedTask;
                }

                yield return Task.FromCanceled(new CancellationToken(true));
            }
        }
    }
}