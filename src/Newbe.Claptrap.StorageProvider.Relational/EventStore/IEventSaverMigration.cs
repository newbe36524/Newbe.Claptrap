using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.Relational.EventStore
{
    public interface IEventSaverMigration
    {
        Task MigrateAsync();
    }
}