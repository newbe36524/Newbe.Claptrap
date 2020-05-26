namespace Newbe.Claptrap.StorageProvider.SQLite.EventStore.OneIdentityOneTable
{
    public static class DbNameHelper
    {
        public static string GetDbNameForOneIdentityOneTable(IClaptrapIdentity identity)
        {
            return $"{identity.TypeCode}_{identity.Id}";
        }
    }
}