using System.Data.SQLite;

namespace Newbe.Claptrap.StorageProvider.SQLite
{
    public class SQLiteDbFactory : ISQLiteDbFactory
    {
        private readonly DbFilePath.Factory _dbFilePathFactory;

        public SQLiteDbFactory(
            DbFilePath.Factory dbFilePathFactory)
        {
            _dbFilePathFactory = dbFilePathFactory;
        }

        public SQLiteConnection GetStateDbConnection(IClaptrapIdentity claptrapIdentity)
        {
            var dbFilePath = _dbFilePathFactory.Invoke(claptrapIdentity);
            var filename = dbFilePath.GetStateDbFilename();
            return new SQLiteConnection(DbHelper.ConnectionString(filename));
        }
    }
}