using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.Relational.EventStore
{
    public interface IEventLoaderMigration
    {
        Task MigrateAsync();
    }
}