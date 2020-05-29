using System.Data;

namespace Newbe.Claptrap.StorageProvider.MySql
{
    public interface IDbFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        string GetConnectionString(string dbName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        IDbConnection GetConnection(string dbName);
    }
}