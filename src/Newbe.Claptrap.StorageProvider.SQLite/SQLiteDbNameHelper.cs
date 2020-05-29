using System.IO;

namespace Newbe.Claptrap.StorageProvider.SQLite
{
    public static class SQLiteDbNameHelper
    {
        public static string OneIdentityOneTableEventStore(
            IClaptrapDesign design,
            IClaptrapIdentity identity)
        {
            var typeCode = design.ClaptrapMasterDesign?.ClaptrapTypeCode ?? design.ClaptrapTypeCode;
            return Path.Combine($"{typeCode}_{identity.Id}", "eventDb.db");
        }

        public static string OneIdentityOneTableStateStore(
            IClaptrapDesign design,
            IClaptrapIdentity identity)
        {
            var typeCode = design.ClaptrapTypeCode;
            return Path.Combine($"{typeCode}_{identity.Id}", "stateDb.db");
        }
    }
}