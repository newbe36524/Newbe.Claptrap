using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.Relational
{
    public interface IStorageMigration
    {
        Task MigrateAsync();
    }
}