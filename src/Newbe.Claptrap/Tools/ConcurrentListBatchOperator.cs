using System;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap
{
    public class ConcurrentListBatchOperator<T> : IBatchOperator<T>
    {
        private readonly BatchOperatorOptions<T> _options;
        private readonly ILogger<ConcurrentListBatchOperator<T>> _logger;

        public delegate ConcurrentListBatchOperator<T> Factory(BatchOperatorOptions<T> options);

        private int _taskCounter = 0;
        private readonly ConcurrentListBatchOperatorWorker<T>[] _workers;

        // ReSharper disable once NotAccessedField.Local
        private readonly IDisposable _logHandler;

        public ConcurrentListBatchOperator(
            BatchOperatorOptions<T> options,
            ConcurrentListBatchOperatorWorker<T>.Factory factory,
            ILogger<ConcurrentListBatchOperator<T>> logger)
        {
            if (options.BufferTime == null || options.BufferTime == TimeSpan.Zero)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.BufferCount == null || options.BufferCount <= 0)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.WorkerCount == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(options));
            }

            _workers = Enumerable.Range(0, options.WorkerCount)
                .Select(x => factory.Invoke(x, options))
                .ToArray();

            _options = options;
            _logger = logger;

            _logHandler = Observable.Interval(TimeSpan.FromSeconds(10))
                .Where(_ => _workers.Any(w => w.WaitingBatchCount != 0))
                .Subscribe(_ =>
                {
                    var sb = new StringBuilder();
                    sb.AppendLine($"{options.DoManyFuncName} worker status:");
                    foreach (var worker in _workers)
                    {
                        sb.AppendLine(
                            $"worker {worker.Id}: waiting task batch = {worker.WaitingBatchCount}, waiting result batch = {worker.WaitingResultCount} ");
                    }

                    _logger.LogInformation(sb.ToString());
                });
        }

        public ValueTask CreateTask(T input)
        {
            var nowValue = Interlocked.Increment(ref _taskCounter);
            var index = nowValue / _options.BufferCount!.Value % _options.WorkerCount;
            var list = _workers[index];
            return list.CreateTask(input);
        }
    }
}