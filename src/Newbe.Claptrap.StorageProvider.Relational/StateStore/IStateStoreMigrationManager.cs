using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.Relational.StateStore
{
    public interface IStateStoreMigrationManager
    {
        Task MigrateAsync();
    }
}