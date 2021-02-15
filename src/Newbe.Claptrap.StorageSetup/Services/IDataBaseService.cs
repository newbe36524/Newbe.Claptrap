using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageSetup
{
    public interface IDataBaseService
    {
        Task StartAsync(DatabaseType databaseType, int preparingSleepInSec);
        Task CleanAsync(DatabaseType databaseType);
    }
}