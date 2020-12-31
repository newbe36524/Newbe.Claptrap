using MySql.Data.MySqlClient;

namespace Newbe.Claptrap.StorageProvider.MySql
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
        MySqlConnection GetConnection(string connectionName);
    }
}