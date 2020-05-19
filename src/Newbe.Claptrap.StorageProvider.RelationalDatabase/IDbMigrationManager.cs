using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase
{
    public interface IDbMigrationManager
    {
        Task Migrate();
    }
}