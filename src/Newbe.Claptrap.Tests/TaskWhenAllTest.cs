using System;
using System.Linq;
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
    }
}