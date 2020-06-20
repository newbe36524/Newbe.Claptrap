using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.StorageProvider.Relational
{
    public class StorageMigrationContainer : IStorageMigrationContainer
    {
        private readonly ILogger<StorageMigrationContainer> _logger;

        private readonly Dictionary<string, Task>
            _tasks = new Dictionary<string, Task>();

        private readonly IDisposable _migrationHandler;
        private readonly Subject<MigrationItem> _subject;

        public StorageMigrationContainer(
            ILogger<StorageMigrationContainer> logger)
        {
            _logger = logger;
            _subject = new Subject<MigrationItem>();
            _migrationHandler = _subject
                .Select(item => Observable.FromAsync(() => MigrateAsync(item)))
                .Concat()
                .Subscribe();
        }

        private Task MigrateAsync(MigrationItem item)
        {
            _logger.LogTrace("{migrationKey} migration coming", item.MigrationKey);
            if (_tasks.TryGetValue(item.MigrationKey, out var task))
            {
                _logger.LogDebug("{migrationKey} task found", item.MigrationKey);
                if (!NeedRestart(task))
                {
                    _logger.LogDebug("{migrationKey} no need restart. current status : {status}",
                        item.MigrationKey,
                        task.Status);
                    return task.ContinueWith(UpdateTcs);
                }
            }

            _logger.LogInformation("{migrationKey} has not started, try to run it", item.MigrationKey);
            _tasks[item.MigrationKey] = item.Tcs.Task;

            var migration = item.Migration;

            void UpdateTcs(Task migrationTask)
            {
                if (migrationTask.IsCompletedSuccessfully)
                {
                    _logger.LogDebug("{migrationKey} success", item.MigrationKey);
                    item.Tcs.SetResult(0);
                }
                else
                {
                    if (migrationTask.IsFaulted)
                    {
                        _logger.LogDebug("{migrationKey} failed", item.MigrationKey);

                        item.Tcs.SetException(migrationTask.Exception.InnerException);
                    }
                    else
                    {
                        _logger.LogDebug("{migrationKey} canceled", item.MigrationKey);
                        item.Tcs.SetCanceled();
                    }
                }
            }

            return migration.MigrateAsync()
                .ContinueWith(UpdateTcs);

            static bool NeedRestart(Task task)
            {
                return task.IsCanceled || task.IsFaulted;
            }
        }

        public Task CreateTask(string migrationKey, IStorageMigration migration)
        {
            var taskCompletionSource = new TaskCompletionSource<int>();
            _subject.OnNext(new MigrationItem
            {
                Migration = migration,
                Tcs = taskCompletionSource,
                MigrationKey = migrationKey
            });
            return taskCompletionSource.Task;
        }

        struct MigrationItem
        {
            public string MigrationKey { get; set; }
            public IStorageMigration Migration { get; set; }
            public TaskCompletionSource<int> Tcs { get; set; }
        }
    }
}