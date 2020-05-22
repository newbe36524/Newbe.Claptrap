using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.StateStore
{
    public interface IStateStoreMigrationManager
    {
        Task MigrateAsync();
    }
}