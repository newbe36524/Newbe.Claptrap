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
            var count = 10_000;
            var tasks = Enumerable.Range(0, count)
                .Select(x => Task.Run(() => Console.Write(1)));

            await tasks.WhenAllComplete(count);
        }

        [Test]
        public void WhenAllCompletedWithError()
        {
            var cc = 10_000;
            Assert.ThrowsAsync<Exception>(() => GetTasks(cc).WhenAllComplete(cc));

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
            var cc = 10_000;
            Assert.ThrowsAsync<TaskCanceledException>(async () => await GetTasks(cc).WhenAllComplete(cc));

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