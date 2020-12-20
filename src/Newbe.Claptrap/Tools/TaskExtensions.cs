using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Newbe.Claptrap
{
    public static class TaskExtensions
    {
        public static Task<int> WhenAllComplete(this IEnumerable<Task> tasks,
            int count,
            Action<int>? stepAction = null)
        {
            var tcs = new TaskCompletionSource<int>();
            var counter = 0;

            foreach (var task in tasks)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await task;
                        var nowValue = Interlocked.Increment(ref counter);
                        stepAction?.Invoke(nowValue);
                        if (nowValue >= count)
                        {
                            tcs.SetResult(nowValue);
                        }
                    }
                    catch (Exception e)
                    {
                        tcs.TrySetException(e);
                    }
                });
            }


            return tcs.Task;
        }
    }
}