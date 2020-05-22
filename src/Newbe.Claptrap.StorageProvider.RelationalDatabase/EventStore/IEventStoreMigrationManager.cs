using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.EventStore
{
    public interface IEventStoreMigrationManager
    {
        Task MigrateAsync();
    }
}