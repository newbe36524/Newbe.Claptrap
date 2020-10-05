using MySql.Data.MySqlClient;
using Newbe.Claptrap.StorageProvider.Relational;

namespace Newbe.Claptrap.StorageProvider.MySql
{
    public class MySqlAdoCache : AdoNetCache<MySqlCommand>, IMySqlAdoCache
    {
        public override int KeyPrefix { get; } = 30_000_000;
    }
}