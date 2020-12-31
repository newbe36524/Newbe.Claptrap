using MySql.Data.MySqlClient;
using Newbe.Claptrap.StorageProvider.Relational;

namespace Newbe.Claptrap.StorageProvider.MySql
{
    public interface IMySqlAdoCache : IAdoNetCache<MySqlCommand>
    {
    }
}