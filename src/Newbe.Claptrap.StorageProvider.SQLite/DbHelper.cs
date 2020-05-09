using System.Data.SQLite;

namespace Newbe.Claptrap.StorageProvider.SQLite
{
    public static class DbHelper
    {
        public static string GetEventTableName(IClaptrapIdentity claptrapIdentity)
        {
            return "events";
        }

        public static string GetStateTableName(IClaptrapIdentity claptrapIdentity)
        {
            return "state";
        }

        public static string ConnectionString(string filename)
        {
            var re =
                $"Data Source={filename};Cache Size=5000;Journal Mode=WAL;Pooling=True;Default IsolationLevel=ReadCommitted";
            return re;
        }

        public static SQLiteConnection CreateInMemoryConnection(IClaptrapIdentity claptrapIdentity)
        {
            var connectionString =
                $"Data Source={claptrapIdentity.TypeCode}_{claptrapIdentity.Id};Mode=Memory;Cache=Shared";
            return new SQLiteConnection(connectionString);
        }
    }
}