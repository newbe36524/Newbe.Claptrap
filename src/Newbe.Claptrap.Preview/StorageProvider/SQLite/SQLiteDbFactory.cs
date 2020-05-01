using System.Data.SQLite;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.StorageProvider.SQLite
{
    public class SQLiteDbFactory : ISQLiteDbFactory
    {
        private readonly DbFilePath.Factory _dbFilePathFactory;

        public SQLiteDbFactory(
            DbFilePath.Factory dbFilePathFactory)
        {
            _dbFilePathFactory = dbFilePathFactory;
        }

        public SQLiteConnection GetEventDbConnection(IClaptrapIdentity claptrapIdentity)
        {
            var dbFilePath = _dbFilePathFactory.Invoke(claptrapIdentity);
            var filename = dbFilePath.GetEventDbFilename();
            return new SQLiteConnection(DbHelper.ConnectionString(filename));
        }

        public SQLiteConnection GetStateDbConnection(IClaptrapIdentity claptrapIdentity)
        {
            var dbFilePath = _dbFilePathFactory.Invoke(claptrapIdentity);
            var filename = dbFilePath.GetStateDbFilename();
            return new SQLiteConnection(DbHelper.ConnectionString(filename));
        }
    }
}