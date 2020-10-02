using System.Data.SQLite;

namespace Newbe.Claptrap.StorageProvider.SQLite
{
    public interface ISQLiteDbFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        string GetConnectionString(string connectionName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="keepOpen"></param>
        /// <returns></returns>
        SQLiteConnection GetConnection(string connectionName, bool keepOpen = false);
    }
}