using MongoDB.Driver;

namespace Newbe.Claptrap.StorageProvider.MongoDB
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
        IMongoClient GetConnection(string connectionName);
    }
}