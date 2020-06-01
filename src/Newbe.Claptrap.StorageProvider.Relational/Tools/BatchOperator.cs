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

        private readonly Subject<SavingItem> _subject = new Subject<SavingItem>();

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
                        foreach (var savingItem in x)
                        {
                            savingItem.Tcs.SetResult(0);
                        }
                    }
                    catch (Exception e)
                    {
                        foreach (var savingItem in x)
                        {
                            savingItem.Tcs.SetException(e);
                        }
                    }
                }))
                .Merge()
                .Subscribe();
        }

        public Task CreateTask(T input)
        {
            var savingItem = new SavingItem
            {
                Tcs = new TaskCompletionSource<int>(),
                Input = input
            };
            _subject.OnNext(savingItem);
            return savingItem.Tcs.Task;
        }

        private struct SavingItem
        {
            public T Input { get; set; }
            public TaskCompletionSource<int> Tcs { get; set; }
        }
    }
}