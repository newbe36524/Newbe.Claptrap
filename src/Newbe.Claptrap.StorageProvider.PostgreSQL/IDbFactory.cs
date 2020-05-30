using System.Data;

namespace Newbe.Claptrap.StorageProvider.PostgreSQL
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