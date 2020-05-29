using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.Relational
{
    public interface IStorageMigrationContainer
    {
        Task CreateTask(string migrationKey, IStorageMigration migration);
    }
}