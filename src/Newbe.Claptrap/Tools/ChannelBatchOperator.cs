using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.AppMetrics;

namespace Newbe.Claptrap
{
    public class ChannelBatchOperator<T> : IBatchOperator<T>
    {
        private readonly BatchOperatorOptions<T> _options;
        private readonly ILogger<ChannelBatchOperator<T>> _logger;
        private readonly IReadOnlyDictionary<string, object>? _cacheData;

        public delegate ChannelBatchOperator<T> Factory(BatchOperatorOptions<T> options);

        private readonly Channel<BatchItem> _channel;

        // ReSharper disable once NotAccessedField.Local
        private readonly Task _task;

        private readonly string _doManyFuncName;

        public ChannelBatchOperator(
            BatchOperatorOptions<T> options,
            ILogger<ChannelBatchOperator<T>> logger)
        {
            if (options.BufferTime == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.BufferCount == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _options = options;
            _logger = logger;
            _doManyFuncName = _options.DoManyFuncName ?? _options.DoManyFunc.ToString();
            if (options.CacheDataFunc != null)
            {
                _cacheData = options.CacheDataFunc.Invoke();
            }

            _channel = Channel.CreateUnbounded<BatchItem>();

            _task = Task.Factory.StartNew(ConsumeTasks, TaskCreationOptions.LongRunning).Unwrap();
        }

        private async Task ConsumeTasks()
        {
            while (true)
            {
                try
                {
                    var items = new List<BatchItem>(_options.BufferCount!.Value);
                    while (await _channel.Reader.WaitToReadAsync())
                    {
                        var last = DateTimeOffset.UtcNow;
                        var windowTime = last.Add(_options.BufferTime!.Value);
                        while (items.Count < _options.BufferCount
                               && windowTime > DateTimeOffset.UtcNow
                               && _channel.Reader.TryRead(out var item))
                        {
                            items.Add(item);
                        }

                        if (items.Any())
                        {
                            try
                            {
                                ClaptrapMetrics.MeasureBatchOperatorGauge(_doManyFuncName, items.Count);
                                ClaptrapMetrics.MeasureBatchOperatorMaxCountGauge(_doManyFuncName, _options.BufferCount!.Value);
                                using var _ = ClaptrapMetrics.MeasureBatchOperatorTime(_doManyFuncName);
                                await DoManyAsync(items);
                            }
                            finally
                            {
                                items.Clear();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "failed to run a batch");
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            }
        }

        private async Task DoManyAsync(List<BatchItem> items)
        {
            try
            {
                var input = items.Select(x => x.Input).ToArray();
                _logger.LogTrace("there are {count} items to do in one batch.", input.Length);
                await _options.DoManyFunc.Invoke(input, _cacheData).ConfigureAwait(false);
                _logger.LogDebug("one batch done with {count} items", input.Length);

                Parallel.ForEach(items.Select(x => x.Vts), tcs =>
                {
                    tcs.SetResult(0);
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "failed to run batch operation");
                Parallel.ForEach(items.Select(x => x.Vts), tcs =>
                {
                    tcs.SetException(e);
                });
            }
        }


        public async ValueTask CreateTask(T input)
        {
            var tcs = new ManualResetValueTaskSource<int>();
            var batchItem = new BatchItem
            {
                Vts = tcs,
                Input = input
            };
            var valueTask = _channel.Writer.WriteAsync(batchItem);
            if (!valueTask.IsCompleted)
            {
                await valueTask;
            }

            var finalValueTask = new ValueTask(batchItem.Vts, tcs.Version);
            if (!finalValueTask.IsCompleted)
            {
                await finalValueTask;
            }
        }

        private struct BatchItem
        {
            public T Input { get; set; }
            public ManualResetValueTaskSource<int> Vts { get; set; }
        }
    }
}