using System.Data;

namespace Newbe.Claptrap.StorageProvider.SQLite
{
    public interface IDbFactory
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
        /// <returns></returns>
        IDbConnection GetConnection(string connectionName);
    }
}