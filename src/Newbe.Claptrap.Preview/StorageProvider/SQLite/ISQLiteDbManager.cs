using System.Data;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.StorageProvider.SQLite
{
    public interface ISQLiteDbManager
    {
        void CreateOrUpdateDatabase(IClaptrapIdentity claptrapIdentity);
    }
}