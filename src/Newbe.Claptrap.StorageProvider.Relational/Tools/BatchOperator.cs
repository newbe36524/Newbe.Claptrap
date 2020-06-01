using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.Relational.Tools
{
    public class BatchOperator<T> : IBatchOperator<T>
    {
        public delegate BatchOperator<T> Factory(BatchOperatorOptions<T> options);

        private readonly Subject<BatchItem> _subject = new Subject<BatchItem>();

        public BatchOperator(
            BatchOperatorOptions<T> options)
        {
            if (options.BufferTime == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.BufferCount == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _subject
                .Buffer(options.BufferTime.Value, options.BufferCount.Value)
                .Where(x => x.Count > 0)
                .Select(x => Observable.FromAsync(async () =>
                {
                    try
                    {
                        await options.DoManyFunc.Invoke(x.Select(a => a.Input)).ConfigureAwait(false);
                        foreach (var batchItem in x)
                        {
                            batchItem.Tcs.SetResult(0);
                        }
                    }
                    catch (Exception e)
                    {
                        foreach (var batchItem in x)
                        {
                            batchItem.Tcs.SetException(e);
                        }
                    }
                }))
                .Merge()
                .Subscribe();
        }

        public Task CreateTask(T input)
        {
            var batchItem = new BatchItem
            {
                Tcs = new TaskCompletionSource<int>(),
                Input = input
            };
            _subject.OnNext(batchItem);
            return batchItem.Tcs.Task;
        }

        private struct BatchItem
        {
            public T Input { get; set; }
            public TaskCompletionSource<int> Tcs { get; set; }
        }
    }
}