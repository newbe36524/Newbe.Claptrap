using System.Data;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.StorageProvider.SQLite
{
    public interface ISQLiteDbManager
    {
        void CreateOrUpdateDatabase(IActorIdentity actorIdentity, IDbConnection dbConnection);
        void DeleteIfFound(IActorIdentity actorIdentity);
    }
}