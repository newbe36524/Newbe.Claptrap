using System.IO;

namespace Newbe.Claptrap.StorageProvider.SQLite.EventStore.OneIdentityOneTable
{
    public static class DbNameHelper
    {
        public static string GetDbNameForOneIdentityOneTable(IClaptrapIdentity identity)
        {
            return Path.Combine($"{identity.TypeCode}_{identity.Id}", "eventDb.db");
        }
    }
}