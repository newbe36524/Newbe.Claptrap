using System;
using System.Buffers;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.ObjectPool;
using NUnit.Framework;
using static Newbe.Claptrap.StorageProvider.SQLite.Tests.SQLiteInsertTest;

namespace Newbe.Claptrap.StorageProvider.SQLite.Tests
{
    public class SQLiteChannelInsertTest
    {
        [Test]
        [TestCase(100)]
        [TestCase(1_000)]
        [TestCase(10_000)]
        [TestCase(100_000)]
        public async Task InsertChannelStructBatchItem(int count)
        {
            const string methodName = nameof(InsertChannelStructBatchItem);
            var file = $"db{count}{methodName}.db";
            var connectionString = DbHelper.ConnectionString(file);
            await ResetDatabase(file);
            const int batchSize = 10000;
            var guid = Guid.NewGuid().ToString();

            var (cmd, p) = CreateCommand();

            await using var sqLiteConnection = new SQLiteConnection(connectionString);
            await sqLiteConnection.OpenAsync();
            cmd.Connection = sqLiteConnection;
            var bounded = Channel.CreateUnbounded<StructBatchItem>();
#pragma warning disable 4014
            Task.Factory.StartNew(async () =>
            {
                var waiting = TimeSpan.FromMilliseconds(50);
                while (await bounded.Reader.WaitToReadAsync())
                {
                    var list = new List<StructBatchItem>();
                    var time = DateTimeOffset.Now;
                    while (list.Count < batchSize
                           && DateTimeOffset.Now - time < waiting
                           && bounded.Reader.TryRead(out var item))
                    {
                        list.Add(item);
                    }

                    if (list.Any())
                    {
                        await using var transaction = await sqLiteConnection.BeginTransactionAsync();
                        foreach (var item in list)
                        {
                            p.Value = item.Data;
                            await cmd.ExecuteNonQueryAsync();
                        }

                        try
                        {
                            await transaction.CommitAsync();
                            foreach (var item in list)
                            {
                                item.Tcs.SetResult(0);
                            }
                        }
                        catch (Exception e)
                        {
                            foreach (var item in list)
                            {
                                item.Tcs.SetException(e);
                            }
                        }
                        finally
                        {
                            list.Clear();
                        }
                    }
                }
            }).Unwrap();
#pragma warning restore 4014

            var pageCount = count / batchSize;
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < pageCount; i++)
            {
                Parallel.For(0, batchSize, async j =>
                {
                    var tcs = new TaskCompletionSource<int>();
                    var valueTask = bounded.Writer.WriteAsync(new StructBatchItem
                    {
                        Tcs = tcs,
                        Data = guid
                    });
                    if (!valueTask.IsCompleted)
                    {
                        await valueTask;
                    }

                    await tcs.Task;
                });
            }

            bounded.Writer.Complete();
            await bounded.Reader.Completion;
            sw.Stop();
            Console.WriteLine($"cost: {sw.ElapsedMilliseconds}");
        }

        [Test]
        [TestCase(100)]
        [TestCase(1_000)]
        [TestCase(10_000)]
        [TestCase(100_000)]
        [TestCase(500_000)]
        public async Task InsertChannelStructBatchItemValueTask(int count)
        {
            const string methodName = nameof(InsertChannelStructBatchItemValueTask);
            var file = $"db{count}{methodName}.db";
            var connectionString = DbHelper.ConnectionString(file);
            await ResetDatabase(file);
            const int batchSize = 10000;
            var guid = Guid.NewGuid().ToString();

            var (cmd, p) = CreateCommand();
            await using var sqLiteConnection = new SQLiteConnection(connectionString);
            await sqLiteConnection.OpenAsync();
            cmd.Connection = sqLiteConnection;
            var bounded = Channel.CreateUnbounded<StructBatchItemValueTask>();
#pragma warning disable 4014
            Task.Factory.StartNew(async () =>
            {
                var waiting = TimeSpan.FromMilliseconds(50);
                while (await bounded.Reader.WaitToReadAsync())
                {
                    var list = new List<StructBatchItemValueTask>();
                    var time = DateTimeOffset.Now;
                    while (list.Count < batchSize
                           && DateTimeOffset.Now - time < waiting
                           && bounded.Reader.TryRead(out var item))
                    {
                        list.Add(item);
                    }

                    if (list.Any())
                    {
                        await using var transaction = await sqLiteConnection.BeginTransactionAsync();
                        foreach (var item in list)
                        {
                            p.Value = item.Data;
                            await cmd.ExecuteNonQueryAsync();
                        }

                        try
                        {
                            await transaction.CommitAsync();
                            foreach (var item in list.AsParallel())
                            {
                                item.Tcs.SetResult(0);
                            }
                        }
                        catch (Exception e)
                        {
                            foreach (var item in list.AsParallel())
                            {
                                item.Tcs.SetException(e);
                            }
                        }
                        finally
                        {
                            list.Clear();
                        }
                    }
                }
            }).Unwrap();
#pragma warning restore 4014

            var pageCount = count / batchSize;
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < pageCount; i++)
            {
                await Enumerable.Range(0, batchSize)
                    .ToObservable()
                    .Select(j => Observable.FromAsync(async () =>
                    {
                        var valueTaskSource = new ManualResetValueTaskSource<int>();
                        var valueTask = bounded.Writer.WriteAsync(new StructBatchItemValueTask
                        {
                            Tcs = valueTaskSource,
                            Data = guid
                        });
                        if (!valueTask.IsCompleted)
                        {
                            await valueTask;
                        }

                        var finalValueTask = new ValueTask(valueTaskSource, valueTaskSource.Version);
                        if (!finalValueTask.IsCompleted)
                        {
                            await finalValueTask;
                        }
                    }))
                    .Merge()
                    .ToTask();
            }

            bounded.Writer.Complete();
            await bounded.Reader.Completion;
            sw.Stop();
            Console.WriteLine($"cost: {sw.ElapsedMilliseconds} ms");
        }

        public struct StructBatchItem
        {
            public string Data { get; set; }
            public TaskCompletionSource<int> Tcs { get; set; }
        }

        public struct StructBatchItemValueTask
        {
            public string Data { get; set; }
            public ManualResetValueTaskSource<int> Tcs { get; set; }
        }

        [Test]
        [TestCase(100)]
        [TestCase(1_000)]
        [TestCase(10_000)]
        [TestCase(100_000)]
        public async Task InsertChannelClassBatchItem(int count)
        {
            const string methodName = nameof(InsertChannelClassBatchItem);
            var file = $"db{count}{methodName}.db";
            var connectionString = DbHelper.ConnectionString(file);
            await ResetDatabase(file);
            const int batchSize = 10000;
            var guid = Guid.NewGuid().ToString();

            var (cmd, p) = CreateCommand();

            await using var sqLiteConnection = new SQLiteConnection(connectionString);
            await sqLiteConnection.OpenAsync();
            cmd.Connection = sqLiteConnection;
            var bounded = Channel.CreateUnbounded<ClassBatchItem>();
#pragma warning disable 4014
            Task.Factory.StartNew(async () =>
            {
                var waiting = TimeSpan.FromMilliseconds(50);
                while (await bounded.Reader.WaitToReadAsync())
                {
                    var list = new List<ClassBatchItem>();
                    var time = DateTimeOffset.Now;
                    while (list.Count < batchSize
                           && DateTimeOffset.Now - time < waiting
                           && bounded.Reader.TryRead(out var item))
                    {
                        list.Add(item);
                    }

                    if (list.Any())
                    {
                        await using var transaction = await sqLiteConnection.BeginTransactionAsync();
                        foreach (var item in list)
                        {
                            p.Value = item.Data;
                            await cmd.ExecuteNonQueryAsync();
                        }

                        try
                        {
                            await transaction.CommitAsync();
                            foreach (var item in list)
                            {
                                item.Tcs.SetResult(0);
                            }
                        }
                        catch (Exception e)
                        {
                            foreach (var item in list)
                            {
                                item.Tcs.SetException(e);
                            }
                        }
                        finally
                        {
                            list.Clear();
                        }
                    }
                }
            }).Unwrap();
#pragma warning restore 4014

            var pageCount = count / batchSize;
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < pageCount; i++)
            {
                Parallel.For(0, batchSize, async j =>
                {
                    var tcs = new TaskCompletionSource<int>();
                    var valueTask = bounded.Writer.WriteAsync(new ClassBatchItem
                    {
                        Tcs = tcs,
                        Data = guid
                    });
                    if (!valueTask.IsCompleted)
                    {
                        await valueTask;
                    }

                    await tcs.Task;
                });
            }

            bounded.Writer.Complete();
            await bounded.Reader.Completion;
            sw.Stop();
            Console.WriteLine($"cost: {sw.ElapsedMilliseconds}");
        }

        [Test]
        [TestCase(100)]
        [TestCase(1_000)]
        [TestCase(10_000)]
        [TestCase(100_000)]
        public async Task InsertChannelClassBatchItemWithPool(int count)
        {
            const string methodName = nameof(InsertChannelClassBatchItemWithPool);
            var file = $"db{count}{methodName}.db";
            var connectionString = DbHelper.ConnectionString(file);
            await ResetDatabase(file);
            const int batchSize = 10000;
            var guid = Guid.NewGuid().ToString();

            var (cmd, p) = CreateCommand();

            await using var sqLiteConnection = new SQLiteConnection(connectionString);
            await sqLiteConnection.OpenAsync();
            cmd.Connection = sqLiteConnection;
            var bounded = Channel.CreateUnbounded<ClassBatchItem>();
            var pool = ObjectPool.Create<ClassBatchItem>();
#pragma warning disable 4014
            Task.Factory.StartNew(async () =>
            {
                var waiting = TimeSpan.FromMilliseconds(50);
                while (await bounded.Reader.WaitToReadAsync())
                {
                    var list = new List<ClassBatchItem>();
                    var time = DateTimeOffset.Now;
                    while (list.Count < batchSize
                           && DateTimeOffset.Now - time < waiting
                           && bounded.Reader.TryRead(out var item))
                    {
                        list.Add(item);
                    }

                    if (list.Any())
                    {
                        await using var transaction = await sqLiteConnection.BeginTransactionAsync();
                        foreach (var item in list)
                        {
                            p.Value = item.Data;
                            await cmd.ExecuteNonQueryAsync();
                        }

                        try
                        {
                            await transaction.CommitAsync();
                            foreach (var item in list)
                            {
                                item.Tcs.SetResult(0);
                            }
                        }
                        catch (Exception e)
                        {
                            foreach (var item in list)
                            {
                                item.Tcs.SetException(e);
                            }
                        }
                        finally
                        {
                            list.Clear();
                            foreach (var classBatchItem in list)
                            {
                                pool.Return(classBatchItem);
                            }
                        }
                    }
                }
            }).Unwrap();
#pragma warning restore 4014

            var pageCount = count / batchSize;
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < pageCount; i++)
            {
                Parallel.For(0, batchSize, async j =>
                {
                    var tcs = new TaskCompletionSource<int>();
                    var classBatchItem = pool.Get();
                    classBatchItem.Tcs = tcs;
                    classBatchItem.Data = guid;
                    var valueTask = bounded.Writer.WriteAsync(classBatchItem);
                    if (!valueTask.IsCompleted)
                    {
                        await valueTask;
                    }

                    await tcs.Task;
                });
            }

            bounded.Writer.Complete();
            await bounded.Reader.Completion;
            sw.Stop();
            Console.WriteLine($"cost: {sw.ElapsedMilliseconds}");
        }

        [Test]
        [TestCase(100)]
        [TestCase(1_000)]
        [TestCase(10_000)]
        [TestCase(100_000)]
        public async Task InsertChannelClassBatchItemSetResult(int count)
        {
            const string methodName = nameof(InsertChannelClassBatchItemSetResult);
            var file = $"db{count}{methodName}.db";
            var connectionString = DbHelper.ConnectionString(file);
            await ResetDatabase(file);
            const int batchSize = 10000;
            var guid = Guid.NewGuid().ToString();

            var (cmd, p) = CreateCommand();

            await using var sqLiteConnection = new SQLiteConnection(connectionString);
            await sqLiteConnection.OpenAsync();
            cmd.Connection = sqLiteConnection;
            var pool = ObjectPool.Create<ClassBatchValueItem>();
            var arrayPool = ArrayPool<ClassBatchValueItem>.Create();
            var resultPool = ObjectPool.Create<ClassBatchValueResultItem>();

            var taskChannel = Channel.CreateUnbounded<ClassBatchValueItem>();
            var resultChannel = Channel.CreateUnbounded<ClassBatchValueResultItem>();
#pragma warning disable 4014
            Task.Factory.StartNew(async () =>
            {
                while (await resultChannel.Reader.WaitToReadAsync())
                {
                    while (resultChannel.Reader.TryRead(out var item))
                    {
                        try
                        {
                            if (item.IsSuccess)
                            {
                                var item1 = item;
                                Parallel.For(0, item.ItemCount, i =>
                                {
                                    var taskCompletionSource = item1.Items[i];
                                    taskCompletionSource.Tcs.SetResult(item1.Result);
                                });
                            }
                            else
                            {
                                var item1 = item;
                                Parallel.For(0, item.ItemCount, i =>
                                {
                                    var taskCompletionSource = item1.Items[i];
                                    taskCompletionSource.Tcs.SetException(item1.Exception);
                                });
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                        finally
                        {
                            resultPool.Return(item);
                            foreach (var task in item.Items)
                            {
                                pool.Return(task);
                            }

                            arrayPool.Return(item.Items);
                        }
                    }
                }
            }, TaskCreationOptions.LongRunning).Unwrap();
#pragma warning restore 4014
#pragma warning disable 4014
            Task.Factory.StartNew(async () =>
            {
                var waiting = TimeSpan.FromMilliseconds(50);
                while (await taskChannel.Reader.WaitToReadAsync())
                {
                    var list = new List<ClassBatchValueItem>();
                    var time = DateTimeOffset.Now;
                    while (list.Count < batchSize
                           && DateTimeOffset.Now - time < waiting
                           && taskChannel.Reader.TryRead(out var item))
                    {
                        list.Add(item);
                    }

                    if (list.Any())
                    {
                        await using var transaction = await sqLiteConnection.BeginTransactionAsync();
                        foreach (var item in list)
                        {
                            p.Value = item.Data;
                            await cmd.ExecuteNonQueryAsync();
                        }

                        try
                        {
                            await transaction.CommitAsync();
                            var batchResultItem = resultPool.Get();
                            batchResultItem.IsSuccess = true;
                            batchResultItem.Result = 0;
                            var classBatchValueItems = arrayPool.Rent(list.Count);
                            list.CopyTo(classBatchValueItems);
                            batchResultItem.Items = classBatchValueItems;
                            batchResultItem.ItemCount = list.Count;
                            var valueTask = resultChannel.Writer.WriteAsync(batchResultItem);
                            if (!valueTask.IsCompleted)
                            {
                                await valueTask;
                            }
                        }
                        catch (Exception e)
                        {
                            var classBatchResultItem = resultPool.Get();
                            classBatchResultItem.IsSuccess = false;
                            classBatchResultItem.Exception = e;
                            var classBatchValueItems = arrayPool.Rent(list.Count);
                            list.CopyTo(classBatchValueItems);
                            classBatchResultItem.Items = classBatchValueItems;
                            classBatchResultItem.ItemCount = list.Count;
                            var valueTask = resultChannel.Writer.WriteAsync(classBatchResultItem);
                            if (!valueTask.IsCompleted)
                            {
                                await valueTask;
                            }
                        }
                        finally
                        {
                            list.Clear();
                        }
                    }
                }
            }).Unwrap();
#pragma warning restore 4014

            var pageCount = count / batchSize;
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < pageCount; i++)
            {
                Parallel.For(0, batchSize, async j =>
                {
                    var classBatchItem = pool.Get();
                    if (classBatchItem.Tcs == null)
                    {
                        classBatchItem.Tcs = new ManualResetValueTaskSource<int>();
                    }
                    else
                    {
                        classBatchItem.Tcs.Reset();
                    }

                    classBatchItem.Data = guid;
                    var valueTask = taskChannel.Writer.WriteAsync(classBatchItem);
                    if (!valueTask.IsCompleted)
                    {
                        await valueTask;
                    }

                    var finalValueTask = new ValueTask(classBatchItem.Tcs, classBatchItem.Tcs.Version);
                    if (!finalValueTask.IsCompleted)
                    {
                        await finalValueTask;
                    }
                });
            }

            taskChannel.Writer.Complete();
            await taskChannel.Reader.Completion;
            sw.Stop();
            Console.WriteLine($"cost: {sw.ElapsedMilliseconds}");
        }

        public class ClassBatchItem
        {
            public string Data { get; set; }
            public TaskCompletionSource<int> Tcs { get; set; }
        }

        public class ClassBatchValueItem
        {
            public string Data { get; set; }
            public ManualResetValueTaskSource<int> Tcs { get; set; }
        }

        public class ClassBatchValueResultItem
        {
            public ClassBatchValueItem[] Items { get; set; }
            public int ItemCount { get; set; }
            public bool IsSuccess { get; set; }
            public Exception Exception { get; set; }
            public int Result { get; set; }
        }
    }
}