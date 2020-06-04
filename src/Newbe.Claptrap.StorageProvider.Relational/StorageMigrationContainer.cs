using System.Collections.Generic;
using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.Relational
{
    public class StorageMigrationContainer : IStorageMigrationContainer
    {
        private readonly Dictionary<string, Task>
            _tasks = new Dictionary<string, Task>();

        private readonly object _locker = new object();

        public Task CreateTask(string migrationKey, IStorageMigration migration)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            if (_tasks.TryGetValue(migrationKey, out var task))
            {
                if (!NeedRestart(task))
                {
                    return task;
                }
            }

            lock (_locker)
            {
                if (_tasks.TryGetValue(migrationKey, out task))
                {
                    if (!NeedRestart(task))
                    {
                        return task;
                    }
                }

                task = migration.MigrateAsync();
                _tasks[migrationKey] = task;
                return task;
            }

            static bool NeedRestart(Task task)
            {
                return task.IsCanceled || task.IsFaulted;
            }
        }
    }
}