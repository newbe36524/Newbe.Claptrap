using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace Newbe.Claptrap.Benchmarks
{
    public class SleepBenchmark
    {
        /// <summary>
        /// sleep in milliseconds
        /// </summary>
        [Params(10, 100, 1000, 5000)]
        public int Milliseconds { get; set; }

        [Benchmark(Description = "Sleep by Task.Delay")]
        public async Task TaskDelay()
        {
            await Task.Delay(Milliseconds);
        }

        [Benchmark(Baseline = true, Description = "Sleep by Thread.Sleep")]
        public void ThreadSleep()
        {
            Thread.Sleep(Milliseconds);
        }
    }
}