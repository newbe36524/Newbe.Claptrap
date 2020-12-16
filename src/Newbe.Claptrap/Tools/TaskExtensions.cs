using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Newbe.Claptrap
{
    public static class TaskExtensions
    {
        public static Task WhenAllComplete(this IEnumerable<Task> tasks)
        {
            var tcs = new TaskCompletionSource<int>();
            var locker = 0;
            IReadOnlyCollection<Task> c;
            if (tasks is IReadOnlyCollection<Task> cc)
            {
                c = cc;
            }
            else
            {
                var taskList = new LinkedList<Task>();
                foreach (var task in tasks)
                {
                    taskList.AddLast(task);
                }

                c = taskList;
            }

            foreach (var task in c)
            {
                task.ContinueWith(ContinuationFunction);
            }

            void ContinuationFunction(Task t)
            {
                var nowValue = Interlocked.Increment(ref locker);
                if (t.IsCompletedSuccessfully)
                {
                    if (nowValue == c.Count)
                    {
                        tcs.SetResult(0);
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