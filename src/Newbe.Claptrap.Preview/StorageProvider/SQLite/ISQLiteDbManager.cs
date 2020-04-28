using System.Data;
using Newbe.Claptrap.Preview.Core;

namespace Newbe.Claptrap.Preview.SQLite
{
    public interface ISQLiteDbManager
    {
        void CreateOrUpdateDatabase(IActorIdentity actorIdentity, IDbConnection dbConnection);
        void DeleteIfFound(IActorIdentity actorIdentity);
    }
}