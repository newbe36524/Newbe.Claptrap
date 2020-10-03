using System;
using System.Buffers;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;

namespace Newbe.Claptrap.MultipleChannelBatchOperators
{
    public class ResultSetter<T>
    {
        private readonly ILogger<MultipleChannelBatchOperator<T>> _logger;
        private readonly Channel<MultipleChannelBatchOperator<T>.BatchItems> _sourceChannel;
        private readonly ArrayPool<MultipleChannelBatchOperator<T>.BatchItem> _itemArrayPool;
        private readonly ObjectPool<MultipleChannelBatchOperator<T>.BatchItem> _itemPool;

        // ReSharper disable once NotAccessedField.Local
        private Task _task = null!;

        public ResultSetter(ILogger<MultipleChannelBatchOperator<T>> logger,
            ObjectPool<MultipleChannelBatchOperator<T>.BatchItem> itemPool,
            Channel<MultipleChannelBatchOperator<T>.BatchItems> sourceChannel)
        {
            _logger = logger;
            _sourceChannel = sourceChannel;
            _itemPool = itemPool;
            _itemArrayPool = ArrayPool<MultipleChannelBatchOperator<T>.BatchItem>.Shared;
        }

        public void Start()
        {
            _task = Task.Factory.StartNew(SetFinalResult, TaskCreationOptions.LongRunning).Unwrap();
        }

        private async Task SetFinalResult()
        {
            while (true)
            {
                try
                {
                    while (await _sourceChannel.Reader.WaitToReadAsync())
                    {
                        var batchItems = await _sourceChannel.Reader.ReadAsync();
                        if (batchItems.Exception != null)
                        {
                            foreach (var batchItem in batchItems.Items[..batchItems.ItemCount])
                            {
                                batchItem.Vts.SetException(batchItems.Exception);
                                _itemPool.Return(batchItem);
                            }
                        }
                        else
                        {
                            foreach (var batchItem in batchItems.Items[..batchItems.ItemCount])
                            {
                                batchItem.Vts.SetResult(0);
                                _itemPool.Return(batchItem);
                            }
                        }

                        _itemArrayPool.Return(batchItems.Items);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "failed to run a batch");
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            }
        }
    }
}