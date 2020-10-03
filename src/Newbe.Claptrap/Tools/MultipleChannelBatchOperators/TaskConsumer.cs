using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.AppMetrics;

namespace Newbe.Claptrap.MultipleChannelBatchOperators
{
    public class TaskConsumer<T>
    {
        private readonly BatchOperatorOptions<T> _options;
        private readonly ILogger<MultipleChannelBatchOperator<T>> _logger;
        private readonly Channel<MultipleChannelBatchOperator<T>.BatchItems> _sourceChannel;
        private readonly Channel<MultipleChannelBatchOperator<T>.BatchItems> _targetChannel;
        private readonly IReadOnlyDictionary<string, object>? _cacheData;
        private readonly string _doManyFuncName;

        // ReSharper disable once NotAccessedField.Local
        private Task _task = null!;

        public TaskConsumer(
            BatchOperatorOptions<T> options,
            ILogger<MultipleChannelBatchOperator<T>> logger,
            Channel<MultipleChannelBatchOperator<T>.BatchItems> sourceChannel,
            Channel<MultipleChannelBatchOperator<T>.BatchItems> targetChannel)
        {
            _options = options;
            _logger = logger;
            _doManyFuncName = _options.DoManyFuncName ?? _options.DoManyFunc.ToString();
            if (options.CacheDataFunc != null)
            {
                _cacheData = options.CacheDataFunc.Invoke();
            }
            _sourceChannel = sourceChannel;
            _targetChannel = targetChannel;
        }

        public void Start()
        {
            _task = Task.Factory.StartNew(ConsumeItems, TaskCreationOptions.LongRunning).Unwrap();
        }
        
        private async Task ConsumeItems()
        {
            while (true)
            {
                try
                {
                    while (await _sourceChannel.Reader.WaitToReadAsync())
                    {
                        var batchItems = await _sourceChannel.Reader.ReadAsync();
                        ClaptrapMetrics.MeasureBatchOperatorGauge(_doManyFuncName, batchItems.ItemCount);
                        ClaptrapMetrics.MeasureBatchOperatorMaxCountGauge(_doManyFuncName, _options.BufferCount!.Value);
                        using var _ = ClaptrapMetrics.MeasureBatchOperatorTime(_doManyFuncName);
                        try
                        {
                            await DoManyAsync(batchItems.Items, batchItems.ItemCount);
                        }
                        catch (Exception e)
                        {
                            batchItems.Exception = e;
                            _logger.LogError(e, "failed to DoManyAsync");
                        }
                        finally
                        {
                            await _targetChannel.Writer.WriteAsync(batchItems);
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
        
        private async Task DoManyAsync(MultipleChannelBatchOperator<T>.BatchItem[] items, int count)
        {
            var input = items[..count].Select(x => x.Input).ToArray();
            _logger.LogTrace("there are {count} items to do in one batch.", input.Length);
            await _options.DoManyFunc.Invoke(input, _cacheData).ConfigureAwait(false);
            _logger.LogDebug("one batch done with {count} items", input.Length);
        }


    }
}