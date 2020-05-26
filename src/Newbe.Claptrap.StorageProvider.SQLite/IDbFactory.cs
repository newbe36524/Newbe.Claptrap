using System.Data;
using Newbe.Claptrap.StorageProvider.Relational;

namespace Newbe.Claptrap.StorageProvider.SQLite
{
    public interface IDbFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbName"></param>
        /// <exception cref="MissingDbException"></exception>
        /// <returns></returns>
        string GetConnectionString(string dbName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbName"></param>
        /// <exception cref="MissingDbException"></exception>
        /// <returns></returns>
        IDbConnection GetConnection(string dbName);
    }

    public class DbFactory : IDbFactory
    {
        public string GetConnectionString(string dbName)
        {
            throw new System.NotImplementedException();
        }

        public IDbConnection GetConnection(string dbName)
        {
            throw new System.NotImplementedException();
        }
    }
}