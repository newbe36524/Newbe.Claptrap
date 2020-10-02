using System.Data.SQLite;
using Newbe.Claptrap.StorageProvider.Relational;

namespace Newbe.Claptrap.StorageProvider.SQLite
{
    public interface ISQLiteAdoNetCache : IAdoNetCache<SQLiteCommand>
    {
    }

    public class SQLiteAdoNetCache : AdoNetCache<SQLiteCommand>, ISQLiteAdoNetCache
    {
        public override int KeyPrefix { get; } = 20_000_000;
    }
}