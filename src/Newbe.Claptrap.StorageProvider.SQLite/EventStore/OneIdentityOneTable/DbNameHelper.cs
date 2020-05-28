using System.IO;

namespace Newbe.Claptrap.StorageProvider.SQLite.EventStore.OneIdentityOneTable
{
    public static class DbNameHelper
    {
        public static string GetDbNameForOneIdentityOneTable(
            IClaptrapDesign design,
            IClaptrapIdentity identity)
        {
            var typeCode = design.ClaptrapMasterDesign?.ClaptrapTypeCode ?? design.ClaptrapTypeCode;
            return Path.Combine($"{typeCode}_{identity.Id}", "eventDb.db");
        }
    }
}