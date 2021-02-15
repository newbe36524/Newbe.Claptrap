using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.StorageProvider.Relational
{
    public class StorageMigrationContainer : IStorageMigrationContainer
    {
        private readonly ILogger<StorageMigrationContainer> _logger;

        private readonly Dictionary<string, Task>
            _tasks = new();

        public StorageMigrationContainer(
            ILogger<StorageMigrationContainer> logger)
        {
            _logger = logger;
        }

        private readonly object _locker = new();

        public Task CreateTask(string migrationKey, IStorageMigration migration)
        {
            return Task.Run(() =>
            {
                _logger.LogTrace("{migrationKey} migration coming", migrationKey);
                if (_tasks.TryGetValue(migrationKey, out var task))
                {
                    _logger.LogTrace("{migrationKey} task found", migrationKey);
                    if (!NeedRestart(task))
                    {
                        _logger.LogTrace("{migrationKey} success", migrationKey);
                        return task;
                    }
                }

                lock (_locker)
                {
                    if (_tasks.TryGetValue(migrationKey, out var task1))
                    {
                        _logger.LogTrace("{migrationKey} task found", migrationKey);
                        if (!NeedRestart(task1))
                        {
                            _logger.LogTrace("{migrationKey} no need restart. current status : {status}",
                                migrationKey,
                                task1.Status);
                            return task1;
                        }
                    }

                    var oneTask = migration.MigrateAsync();
                    _tasks[migrationKey] = oneTask;
                    return oneTask;
                }
            });

            static bool NeedRestart(Task t)
            {
                if (t.IsCompletedSuccessfully)
                {
                    return false;
                }

                if (t.IsFaulted)
                {
                    return true;
                }

                return false;
            }
        }
    }
}