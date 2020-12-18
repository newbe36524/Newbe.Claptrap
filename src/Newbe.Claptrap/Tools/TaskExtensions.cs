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
                task.ContinueWith(ContinuationFunction);
            }

            void ContinuationFunction(Task t)
            {
                var nowValue = Interlocked.Increment(ref counter);
                stepAction?.Invoke(nowValue);
                if (t.IsCompletedSuccessfully)
                {
                    if (nowValue >= count)
                    {
                        tcs.SetResult(nowValue);
                    }
                }
                else if (t.IsFaulted)
                {
                    tcs.TrySetException(t.Exception!);
                }
                else if (t.IsCanceled)
                {
                    tcs.TrySetCanceled();
                }
            }


            return tcs.Task;
        }
    }
}